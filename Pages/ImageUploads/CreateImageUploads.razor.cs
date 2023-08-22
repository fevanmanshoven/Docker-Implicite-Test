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

namespace ImpliciteTesterServer.Pages.ImageUploads
{
    public partial class CreateImageUploads
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }

        private List<ImageUpload> imageUploads = new();
        private IReadOnlyList<IBrowserFile> imageFiles;
        private List<Category> categories = new();
        private MyDbContext context = new MyDbContext();


        private IEnumerable<Category> selectedCategories { get; set; } = new HashSet<Category>() { };


        private MudForm form;

        bool loading = true;
        bool loadingUpload = false;

        bool success;
        string[] errors = { };

        ImageUpload newImageUpload = new();
        List<Image> newImages = new();
        String newImageNames = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                getCategories();
                getImageUploads();
            }
            catch
            {

            }
            
            loading = false;
        }

        private void NavigateToDetailsImageUploads(int uploadImageId)
        {
            nav.NavigateTo("details-upload/" + uploadImageId);
        }

        protected void getCategories()
        {
            categories = context.Categories.ToList();
        }

        //protected async Task getCategories()
        //{
        //    var trustedFileNameForFileStorage = "Categories.json";
        //    var path = Path.Combine(env.WebRootPath,
        //            "Data",
        //            "Classes",
        //            trustedFileNameForFileStorage);
        //    await using FileStream fs = new(path, FileMode.Open);
        //    categories = (await JsonSerializer.DeserializeAsync<Category[]>(fs)).ToList();
        //}

        protected void getImageUploads()
        {
            Snackbar.Add("Images loading...");
            imageUploads = context.ImageUploads.Include(i => i.Categories).Include(i => i.Images).ToList();
            Snackbar.Add("Images loaded...");
        }

        //protected async Task getImages()
        //{
        //    var trustedFileNameForFileStorage = "Images.json";
        //    var path = Path.Combine(env.WebRootPath,
        //            "Data",
        //            "Classes",
        //            trustedFileNameForFileStorage);
        //    await using FileStream fs = new(path, FileMode.Open);
        //    imageUploads = (await JsonSerializer.DeserializeAsync<ImageUpload[]>(fs)).ToList();
        //}

        private async Task SubmitAsync()
        {
            await form.Validate();
            if (success)
            {
                newImageUpload.Categories = selectedCategories.ToList();
                try
                {
                    await SaveFile();
                    await SaveAsync();
                    Snackbar.Add("Item added");
                    newImageUpload = new();
                }
                catch(Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                    imageFiles = null;
                    newImageUpload.Images = new();
                }
                
            }
            else
            {
                Snackbar.Add("Something went wrong", Severity.Error);
            }
        }

        private void SelectFile(InputFileChangeEventArgs e)
        {
            imageFiles = e.GetMultipleFiles();
            newImageNames = (string.Join(", ", e.GetMultipleFiles().Select(f => f.Name)));
            Snackbar.Add(newImageNames);
            Snackbar.Add("Selected");
        }

        private async Task SaveFile()
        {
            loadingUpload = true;
            var count = 0;
            foreach (IBrowserFile newFile in imageFiles)
            {
                var fileExtension = newFile.Name.Substring(newFile.Name.LastIndexOf("."));
                var trustedFileNameForFileStorage = newImageUpload.Name +"_"+ count+fileExtension;
                var src = Path.Combine("Data",
                        "Images",
                        trustedFileNameForFileStorage);
                var path = Path.Combine(env.WebRootPath,src);
                Image newImage = new(newFile.Name,src,path,newImageUpload);
                try
                {
                    await using FileStream fs = new(path, FileMode.Create);
                    await using var input = newFile.OpenReadStream(10000000); // 10mb
                    await input.CopyToAsync(fs);
                    newImages.Add(newImage);
                    count += 1;
                    Snackbar.Add("ImageUpload file saved");

                }
                catch (Exception ex)
                {

                    Snackbar.Add(("Something went wrong: File:" + newFile.Name + " Error: " + ex.Message), Severity.Error);
                    Snackbar.Add(("The maximum image size is 10 megabytes"), Severity.Error);
                    File.Delete(path);
                    loadingUpload = false;
                    throw new IOException(ex.Message);
                }
            }
            loadingUpload = false;
        }

        private async Task SaveAsync()
        {
            loadingUpload = true;
            try
            {
                newImageUpload.Images = newImages;
                imageUploads.Add(newImageUpload);
                context.ImageUploads.Add(newImageUpload);
                await context.SaveChangesAsync();
                Snackbar.Add("Images saved");
                newImageUpload = new();
                newImages = new();
                newImageNames = "";
                selectedCategories = new HashSet<Category>() { };
            }
            catch (Exception ex)
            {
                loadingUpload = false;
                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                throw new IOException(ex.Message);
            }
            loadingUpload = false;
        }

        //private async Task SaveAsync()
        //{
        //    loadingUpload = true;
        //    var trustedFileNameForFileStorage = "Images.json";
        //    var path = Path.Combine(env.WebRootPath,
        //            "Data",
        //            "Classes",
        //            trustedFileNameForFileStorage);
        //    try
        //    {

        //        await using FileStream fs = new(path, FileMode.Create);
        //        await JsonSerializer.SerializeAsync(fs, imageUploads);
        //        Snackbar.Add("Images saved");
        //    }
        //    catch (Exception ex)
        //    {
        //        loadingUpload = false;
        //        Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
        //        throw new IOException(ex.Message);
        //    }
        //    loadingUpload = false;
        //}

        private async Task Remove(ImageUpload imageUpload)
        {
            foreach(Image image in imageUpload.Images)
            {
                File.Delete(image.Path);
            }
            imageUploads.Remove(imageUpload);
            context.ImageUploads.Remove(imageUpload);
            await context.SaveChangesAsync();
        }
    }
}



