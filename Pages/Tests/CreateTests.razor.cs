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

namespace ImpliciteTesterServer.Pages.Tests
{
    public partial class CreateTests
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }

        private List<Test> tests = new();
        private MyDbContext context = new MyDbContext();

        private List<Category> categories = new();
        private List<ImageUpload> imageUploads = new();

        private IEnumerable<Category> selectedPosCategories { get; set; } = new HashSet<Category>() { };
        private IEnumerable<ImageUpload> selectedPosImageUploads { get; set; } = new HashSet<ImageUpload>() { };

        private IEnumerable<Category> selectedNegCategories { get; set; } = new HashSet<Category>() { };
        private IEnumerable<ImageUpload> selectedNegImageUploads { get; set; } = new HashSet<ImageUpload>() { };


        private MudForm form;

        bool loading = true;
        bool loadingUpload = false;

        bool success;
        string[] errors = { };

        Test newTest = new("");

        protected override async Task OnInitializedAsync()
        {
            try
            {
                getCategories();
                getImageUploads();
                getTests();
            }
            catch
            {

            }
            
            loading = false;
        }

        private void NavigateToResultTests(int testId)
        {
            nav.NavigateTo("results/" + testId);
        }

        private void NavigateToDetailsTests(int testId)
        {
            nav.NavigateTo("details-test/" + testId);
        }

        private void NavigateToRunTests(int testId)
        {
            nav.NavigateTo("run-test/" + testId);
        }

        protected void getCategories()
        {
            Snackbar.Add("Category loading...");
            categories = context.Categories.ToList();
        }

        protected void getImageUploads()
        {
            Snackbar.Add("ImageUploads loading...");
            imageUploads = context.ImageUploads.ToList();
        }

        protected void getTests()
        {
            Snackbar.Add("Tests loading...");
            tests = context.Tests.Include(t => t.Fases).Include(t => t.Results).ToList();
        }

        private async Task SubmitAsync()
        {
            await form.Validate();
            if (success)
            {
                newTest.PosCategories = selectedPosCategories.ToList();
                newTest.PosImageUploads = selectedPosImageUploads.ToList();
                newTest.NegCategories = selectedNegCategories.ToList();
                newTest.NegImageUploads = selectedNegImageUploads.ToList();

                try
                {
                    await SaveAsync();
                    Snackbar.Add("Item added");
                    newTest = new("");
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

        private async Task SaveAsync()
        {
            loadingUpload = true;
            try
            {
                context.Tests.Add(newTest);
                await context.SaveChangesAsync();
                getTests();
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

        private async Task Remove(Test test)
        {
            foreach(Fase fase in test.Fases)
            {
                context.Remove(fase);
            }
            tests.Remove(test);
            context.Tests.Remove(test);
            await context.SaveChangesAsync();
        }
    }
}



