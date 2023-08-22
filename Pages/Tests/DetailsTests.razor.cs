using System;
using System.IO;
using System.Text.Json;
using ImpliciteTesterServer.Data;
using ImpliciteTesterServer.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace ImpliciteTesterServer.Pages.Tests
{
    public partial class DetailsTests
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Parameter] public string TestId { get; set; }

        private List<Category> categories = new();
        private List<ImageUpload> imageUploads = new();

        private IEnumerable<Category> selectedPosCategories { get; set; } = new HashSet<Category>() { };
        private IEnumerable<ImageUpload> selectedPosImageUploads { get; set; } = new HashSet<ImageUpload>() { };

        private IEnumerable<Category> selectedNegCategories { get; set; } = new HashSet<Category>() { };
        private IEnumerable<ImageUpload> selectedNegImageUploads { get; set; } = new HashSet<ImageUpload>() { };

        private Test test = new("");
        private List<Fase> fases = new();

        private Fase newFase = new("");
        private Array faseTypes = Enum.GetValues(typeof(FaseType));
        private IEnumerable<FaseType> selectedFasetypes { get; set; } = new HashSet<FaseType>() { FaseType.Mixed };

        private static Random rng = new Random();

        private MudForm testForm;
        private MudForm faseForm;


        private MyDbContext context = new MyDbContext();

        bool loading = true;
        bool loadingUpload = false;

        bool success;
        string[] errors = { };

        protected override async Task OnInitializedAsync()
        {
            getTests(Int16.Parse(TestId));
            getCategories();
            loading = false;
        }

        protected void getTests(int testId)
        {
            Snackbar.Add("Test loading...");
            test = context.Tests.Where(t => t.TestId == testId)
                .Include(i => i.PosCategories)
                .Include(i => i.PosImageUploads)
                .Include(i => i.NegCategories)
                .Include(i => i.NegImageUploads)
                .Include(i => i.Results)
                .Include(i => i.Fases).ThenInclude(i => i.FaseTypeImages).ThenInclude(f => f.Image).FirstOrDefault();
            selectedPosCategories = test.PosCategories.ToHashSet();
            selectedPosImageUploads = test.PosImageUploads.ToHashSet();
            selectedNegCategories = test.NegCategories.ToHashSet();
            selectedNegImageUploads = test.NegImageUploads.ToHashSet();

            fases = test.Fases;

            Snackbar.Add("Test loaded...");
        }

        protected void getCategories()
        {
            categories = context.Categories.ToList();
        }

        protected List<FaseTypeImage> getPosFaseImageUploads(int amount)
        {
            List<Category> posCategories = selectedPosCategories.ToList();

            List<FaseTypeImage> posFaceTypeImages = new();
            List<string> srcs = new();
            foreach (Category cat in posCategories)
            {
                var posImageUplaods = context.ImageUploads.Where(iu => iu.Categories.Contains(cat));
                if (selectedPosImageUploads.Any())
                {
                    List<int> posImageUploadIDs = selectedPosImageUploads.Select(iu => iu.ImageUploadId).ToList();
                    posImageUplaods = posImageUplaods.Where(iu => posImageUploadIDs.Contains(iu.ImageUploadId));
                }
                var posFoundImages = posImageUplaods.SelectMany(i => i.Images);
                var posFoundFaseTypeImages = posFoundImages.Select(i => new FaseTypeImage(i, FaseType.Positive)).ToList();

                posFaceTypeImages.AddRange(posFoundFaseTypeImages);
            }

            posFaceTypeImages = Shuffle(posFaceTypeImages);

            return posFaceTypeImages.Take(amount).ToList();
        }

        protected List<FaseTypeImage> getNegFaseImageUploads(int amount)
        {
            List<Category> negCategories = selectedNegCategories.ToList();

            List<FaseTypeImage> negFaceTypeImages = new();
            List<string> srcs = new();
            foreach (Category cat in negCategories) {

                var ImageUplaods = context.ImageUploads.Where(iu => iu.Categories.Contains(cat));
                if (selectedNegImageUploads.Any())
                {
                    List<int> negImageUploadIDs = selectedNegImageUploads.Select(iu => iu.ImageUploadId).ToList();
                    ImageUplaods = ImageUplaods.Where(iu => negImageUploadIDs.Contains(iu.ImageUploadId));
                }
                var foundImages = ImageUplaods.SelectMany(i => i.Images);
                var foundFaseTypeImages = foundImages.Select(i => new FaseTypeImage(i, FaseType.Negative)).ToList();

                negFaceTypeImages.AddRange(foundFaseTypeImages);
            }

            negFaceTypeImages = Shuffle(negFaceTypeImages);

            return negFaceTypeImages.Take(amount).ToList();
        }

        private async Task SubmitTestAsync()
        {
            await testForm.Validate();
            if (success)
            {
                test.PosCategories = selectedPosCategories.ToList();
                test.PosImageUploads = selectedPosImageUploads.ToList();
                test.NegCategories = selectedNegCategories.ToList();
                test.NegImageUploads = selectedNegImageUploads.ToList();

                try
                {
                    await SaveTestAsync();
                    Snackbar.Add("Item saved");
                }
                catch (Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                }

            }
            else
            {
                Snackbar.Add("Something went wrong", Severity.Error);
            }
        }

        private async Task SubmitFaseAsync()
        {
            await faseForm.Validate();
            if (success)
            {
                newFase.FaseType = selectedFasetypes.FirstOrDefault();
                switch (newFase.FaseType)
                {
                    case FaseType.Positive:
                        newFase.FaseTypeImages = getPosFaseImageUploads(newFase.ImgAmount);
                        break;

                    case FaseType.Negative:
                        newFase.FaseTypeImages = getNegFaseImageUploads(newFase.ImgAmount);
                        break;

                    case FaseType.Mixed:
                        int unevenCheck = newFase.ImgAmount % 2 == 0 ? newFase.ImgAmount / 2 : (newFase.ImgAmount / 2) + 1;
                        var posImages = getPosFaseImageUploads(newFase.ImgAmount / 2);
                        var negImages = getNegFaseImageUploads(unevenCheck);
                        var combinedImages = posImages;
                        combinedImages.AddRange(negImages);
                        newFase.FaseTypeImages = Shuffle(combinedImages);
                        break;

                    default: break;
                }
                try
                {
                    await SaveFaseAsync();
                    Snackbar.Add("Item saved");
                }
                catch (Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                }

            }
            else
            {
                Snackbar.Add("Something went wrong", Severity.Error);
            }
        }

        private async Task SaveFaseAsync()
        {
            loadingUpload = true;
            try
            {
                test.Fases.Add(newFase);
                test.PosCategories = selectedPosCategories.ToList();
                test.PosImageUploads = selectedPosImageUploads.ToList();
                test.NegCategories = selectedNegCategories.ToList();
                test.NegImageUploads = selectedNegImageUploads.ToList();
                context.Tests.Update(test);
                context.Fases.Add(newFase);
                await context.SaveChangesAsync();
                StateHasChanged();
                Snackbar.Add("Fase added");
                newFase = new Fase("");
            }
            catch (Exception ex)
            {
                loadingUpload = false;
                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                throw new IOException(ex.Message);
            }
            loadingUpload = false;
        }


        private async Task SaveTestAsync()
        {
            loadingUpload = true;
            try
            {
                context.Tests.Update(test);
                await context.SaveChangesAsync();
                Snackbar.Add("Test saved");
            }
            catch (Exception ex)
            {
                loadingUpload = false;
                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                throw new IOException(ex.Message);
            }
            loadingUpload = false;
        }

        private async Task Remove(Fase fase)
        {
            fases.Remove(fase);
            context.Fases.Remove(fase);
            await context.SaveChangesAsync();
        }

        public List<FaseTypeImage> Shuffle(List<FaseTypeImage> list)
        {
            List<FaseTypeImage> shuffled = list;
            int n = shuffled.Count;
            while (n > 1)
            {
                n--;
                shuffled = shuffled.OrderBy(a => rng.Next()).ToList();
            }
            return shuffled;
        }
    }

}



