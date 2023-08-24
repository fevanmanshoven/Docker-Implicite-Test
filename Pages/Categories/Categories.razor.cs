using System;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using DockerImpliciteTest.Data;
using DockerImpliciteTest.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using MudBlazor;
using static System.Net.WebRequestMethods;

namespace DockerImpliciteTest.Pages.Categories
{
    public partial class Categories
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] HttpClient Http { get; set; }


        private MyDbContext context = new MyDbContext();
        private List<Category> categories = new();
        private MudForm form;

        bool loading = true;
        bool success;
        string[] errors = { };

        Category newCategory = new();

        protected override void OnInitialized()
        {
            try
            {
                getCategories();
            }
            catch (Exception ex)
            {
                Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
            }

            loading = false;
        }

        protected void getCategories()
        {
            Snackbar.Add("Category loading...");
            categories = context.Categories.ToList();
            Snackbar.Add("Category loaded...");

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

        private async void Submit()
        {
            await form.Validate();
            if (success)
            {
                try
                {
                    //await SaveFile();
                    categories.Add(newCategory);
                    Snackbar.Add("Category added");
                    // Snackbar.Add(categories.ToString());
                    await SaveAsync();
                    newCategory = new();
                }
                catch(Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                    //newCategory.ImageFile = null;
                    //newCategory.ImageName = null;
                }

            }
            else
            {
                Snackbar.Add("Something went wrong", Severity.Error);
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                context.Categories.Add(newCategory);
                await context.SaveChangesAsync();
                Snackbar.Add("Categories saved");
            }
            catch (Exception ex)
            {

                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                //throw new IOException(ex.Message);
            }
        }

        private async Task Remove(Category category)
        {
            var usedCategoriesImageUploads = context.ImageUploads.SelectMany(i => i.Categories);
            var usedCategoriesTestsPostive = context.Tests.SelectMany(i => i.PosCategories);
            var usedCategoriesTestsNegative = context.Tests.SelectMany(i => i.NegCategories);

            if (usedCategoriesImageUploads.Contains(category))
            {
                Snackbar.Add(("Category is in use by an imageupload"), Severity.Error);
            }else if (usedCategoriesTestsPostive.Contains(category) || usedCategoriesTestsNegative.Contains(category))
            {
                Snackbar.Add(("Category is in use by a test"), Severity.Error);
            }
            else
            {
                categories.Remove(category);
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                Snackbar.Add("Categorie deleted");
            }
        }

        //private async Task SaveAsync()
        //{

        //    var trustedFileNameForFileStorage = "Categories.json";
        //    var path = Path.Combine(env.WebRootPath,
        //            "Data",
        //            "Classes",
        //            trustedFileNameForFileStorage);
        //    try
        //    {

        //        await using FileStream fs = new(path, FileMode.Create);
        //        await JsonSerializer.SerializeAsync(fs, categories);
        //        Snackbar.Add("Categories saved");
        //    }
        //    catch (Exception ex)
        //    {

        //        Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
        //        //throw new IOException(ex.Message);
        //    }
        //}

    }
}



