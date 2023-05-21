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

namespace ImpliciteTesterServer.Pages.Results
{
    public partial class ViewResults
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }

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
                getResults();
            }
            catch
            {

            }
            
            loading = false;
        }

        private void NavigateToDetailsResult(int result)
        {
            nav.NavigateTo("details-result/" + result);
        }

        protected void getResults()
        {
            Snackbar.Add("Results loading...");
            results = context.Results.Include(r => r.Test).ToList();
        }

        private async Task Remove(Result result)
        {
            results.Remove(result);
            context.Results.Remove(result);
            await context.SaveChangesAsync();
        }
    }
}



