using System;
using System.IO;
using System.Text.Json;
using DockerImpliciteTest.Data;
using DockerImpliciteTest.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace DockerImpliciteTest.Pages.Results
{
    public partial class ViewResultsFromTest
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Parameter] public string TestId { get; set; }
        private List<Result> results = new();
        private MyDbContext context = new MyDbContext();


        private MudForm form;

        bool loading = true;
        bool loadingUpload = false;

        bool success;
        string[] errors = { };

        protected override async Task OnInitializedAsync()
        {
            try
            {
                getResults(Int16.Parse(TestId));
            }
            catch
            {

            }
            
            loading = false;
        }

        private void NavigateToDetailsTests(int testId)
        {
            nav.NavigateTo("details-result/" + testId);
        }

        private void NavigateToAddResult()
        {
             nav.NavigateTo("create-result/" + Int16.Parse(TestId));
        }

        protected void getResults(int testId)
        {
            Snackbar.Add("Results loading...");
            results = context.Results.Where(r => r.Test.TestId == testId).Include(r => r.Test).ToList();
            Snackbar.Add("Results loaded...");

        }

        private async Task Remove(Result result)
        {
            results.Remove(result);
            context.Results.Remove(result);
            await context.SaveChangesAsync();
        }
    }
}



