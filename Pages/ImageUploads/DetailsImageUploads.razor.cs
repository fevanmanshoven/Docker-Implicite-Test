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
    public partial class DetailsImageUploads
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Parameter] public string ImageUploadId { get; set; }

        private List<Category> categories = new();
        private IEnumerable<Category> selectedCategories { get; set; } = new HashSet<Category>() { };

        private IReadOnlyList<IBrowserFile> imageFiles;

        private ImageUpload imageUpload = new();
        private List<Image> images = new();

        private MudForm form;

        private MyDbContext context = new MyDbContext();

        String newImageNames = "";

        bool loading = true;
        bool loadingUpload = false;

        bool success;
        string[] errors = { };

        protected override async Task OnInitializedAsync()
         {
            getImageUploads(Int16.Parse(ImageUploadId));
            getCategories();
            loading = false;
        }

        protected void getImageUploads(int imageUploadId)
        {
            Snackbar.Add("Images loading...");
            imageUpload = context.ImageUploads.Where(ip => ip.ImageUploadId == imageUploadId).Include(i => i.Categories).FirstOrDefault();
            selectedCategories = imageUpload.Categories.ToHashSet();
            images = context.Images.Where(i => i.ImageUpload == imageUpload).ToList();

        }

        protected void getCategories()
        {
            categories = context.Categories.ToList();
        }

        private async Task SubmitAsync()
        {
            await form.Validate();
            if (success)
            {
                imageUpload.Categories = selectedCategories.ToList();
                try
                {
                    if (imageFiles != null)
                    {
                        await SaveFile();
                    }
                    await SaveAsync();
                    Snackbar.Add("Item added");
                }
                catch (Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                    imageFiles = null;
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
            var count = images.Count();
            foreach (IBrowserFile newFile in imageFiles)
            {
                var fileExtension = newFile.Name.Substring(newFile.Name.IndexOf("."));
                var trustedFileNameForFileStorage = imageUpload.Name + "_" + count + fileExtension;
                var src = Path.Combine("Data",
                        "Images",
                        trustedFileNameForFileStorage);
                var path = Path.Combine(env.WebRootPath, src);
                Image newImage = new(newFile.Name, src, path, imageUpload);
                try
                {
                    await using FileStream fs = new(path, FileMode.Create);
                    await using var input = newFile.OpenReadStream(10000000); // 10mb
                    await input.CopyToAsync(fs);
                    images.Add(newImage);
                    context.Images.Add(newImage);
                    await context.SaveChangesAsync();
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
                context.ImageUploads.Update(imageUpload);
                await context.SaveChangesAsync();
                Snackbar.Add("Images saved");   
            }
            catch (Exception ex)
            {
                loadingUpload = false;
                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                throw new IOException(ex.Message);
            }
            loadingUpload = false;
        }

        private async Task Remove(Image image)
        {
            File.Delete(image.Path);
            images.Remove(image);
            context.Images.Remove(image);
            await context.SaveChangesAsync();
        }
    }
}



