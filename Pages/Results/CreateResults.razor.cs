using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using DockerImpliciteTest.Data;
using DockerImpliciteTest.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace DockerImpliciteTest.Pages.Results
{
    public partial class CreateResults
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }


        [Parameter] public string TestId { get; set; }

        private Test test = new("");
        private MyDbContext context = new MyDbContext();

        private List<Category> categories = new();
        private List<ImageUpload> imageUploads = new();

        private DateTime startTime;
        private DateTime endTime;

        private List<String> timeLineResult = new();

        private string showingSrc = "";
        private string countdownNumber = "";
        private int countdownSeconds = 0;

        private Test loadedTest = new("");
        private Result newResult = new("");
        private Result processedResult = new("");


        private MudForm form;

        String newFaceReaderFileNames = "";
        private IReadOnlyList<IBrowserFile> FaceReaderFile;

        bool loading = true;
        bool loadingUpload = false;
        bool isStarted = false;
        bool isCountdown = false;
        bool isFinished = false;


        bool success;
        string[] errors = { };

        

        protected override async Task OnInitializedAsync()
         {
            getTest(Int16.Parse(TestId));
            loading = false;
        }

                protected void getTest(int testId)
        {
            Snackbar.Add("Test loading...");
            loadedTest = context.Tests.Where(t => t.TestId == testId).FirstOrDefault();
        }

                private void SelectFile(InputFileChangeEventArgs e)
        {
            FaceReaderFile = e.GetMultipleFiles();
            newFaceReaderFileNames = (string.Join(", ", e.GetMultipleFiles().Select(f => f.Name)));
            Snackbar.Add(newFaceReaderFileNames);
            Snackbar.Add("Selected");
        }

        private async Task SubmitAsync()
        {
            await form.Validate();
            if (success)
            {
                newResult.Test = loadedTest;
                newResult.TimeLineResult = timeLineResult.ToArray();
                try
                {
                    await SaveFile();
                    await SaveAsync();
                    Snackbar.Add("Item added");
                    processedResult = newResult;
                    processFaceData();
                    StateHasChanged();
                    newResult = new("");
                }
                catch (Exception ex)
                {
                    Snackbar.Add("Something went wrong: " + ex.ToString(), Severity.Error);
                    FaceReaderFile = null;
                    newResult.FaceReader = new();
                }

            }
            else
            {
                Snackbar.Add("Something went wrong", Severity.Error);
            }
        }

        private async Task SaveFile()
        {
            loadingUpload = true;
            var count = 0;
            foreach (IBrowserFile newFile in FaceReaderFile)
            {
                var fileExtension = newFile.Name.Substring(newFile.Name.LastIndexOf("."));
                var trustedFileNameForFileStorage = newResult.Name + "_" + count + fileExtension;
                var src = Path.Combine("Data",
                        "FaceReader",
                        trustedFileNameForFileStorage);
                var path = Path.Combine(env.WebRootPath, src);
                FaceReader faceReader = new(newFile.Name, src, path);
                try
                {
                    await using FileStream fs = new(path, FileMode.Create);
                    await using var input = newFile.OpenReadStream(10000000); // 10mb
                    await input.CopyToAsync(fs);
                    newResult.FaceReader = faceReader;
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
                newResult.Test = loadedTest;
                newResult.TimeLineResult = timeLineResult.ToArray();
                context.Results.Add(newResult);
                await context.SaveChangesAsync();
                Snackbar.Add("Result saved");
            }
            catch (Exception ex)
            {
                loadingUpload = false;
                Snackbar.Add(("Something went wrong:" + " Error:" + ex.Message), Severity.Error);
                throw new IOException(ex.Message);
            }
            loadingUpload = false;
        }

        private void processFaceData()
        {
            List<FaceReaderData> faceReaderDatas = new();
            bool isFirstRow = true;

            DataTable datatable = new DataTable();
            StreamReader streamreader = new StreamReader(processedResult.FaceReader.Path);
            char[] delimiter = new char[] { '\t' };
            string[] columnheaders = streamreader.ReadLine().Split(delimiter);
            foreach (string columnheader in columnheaders)
            {
                datatable.Columns.Add(columnheader); // I've added the column headers here.
            }

            while (streamreader.Peek() > 0)
            {
                DataRow datarow = datatable.NewRow();
                datarow.ItemArray = streamreader.ReadLine().Split(delimiter);
                datatable.Rows.Add(datarow);
            }

            int timetracker = 1;
            List<double> totalNeutral = new();
            List<double> totalHappy = new();
            List<double> totalSad = new();
            List<double> totalAngry = new();
            List<double> totalSurprised = new();
            List<double> totalScared = new();
            List<double> totalDisgusted = new();
            List<double> totalContempt = new();
            double videoTime = 0;
            foreach (DataRow row in datatable.Rows)
            {
                if (videoTime > timetracker)
                {
                    FaceReaderData faceReaderData = new(timetracker, totalNeutral.Average(), totalHappy.Average(), totalSad.Average(), totalAngry.Average(), totalSurprised.Average(), totalScared.Average(), totalDisgusted.Average(), totalContempt.Average());
                    faceReaderDatas.Add(faceReaderData);
                    timetracker += 1;
                    totalNeutral = new();
                    totalHappy = new();
                    totalSad = new();
                    totalAngry = new();
                    totalSurprised = new();
                    totalScared = new();
                    totalDisgusted = new();
                    totalContempt = new();
                }
                foreach (DataColumn column in datatable.Columns)
                {
                    bool success;
                    switch (column.ColumnName)
                    {
                        case "Video Time":
                            var time = row[column];
                            videoTime = TimeSpan.Parse(row[column].ToString()).TotalSeconds;
                            break;
                        case "Neutral":
                            string neutral = (string)row[column];
                            double neutralparse;
                            success = double.TryParse(neutral, out neutralparse); 
                            if(success){
                                totalNeutral.Add(Convert.ToDouble(neutralparse));
                            }else{
                                totalNeutral.Add(0.0);
                            }
                            break;
                        case "Happy":
                            var happy = (string)row[column];
                            double happyparse;
                            success = double.TryParse(happy, out happyparse); 
                            if(success){
                                totalHappy.Add(Convert.ToDouble(happyparse));
                            }else{
                                totalHappy.Add(0.0);
                            }
                            break;
                        case "Sad":
                            var sad = (string)row[column];
                            double sadparse;
                            success = double.TryParse(sad, out sadparse); 
                            if(success){
                                totalSad.Add(Convert.ToDouble(sadparse));
                            }else{
                                totalSad.Add(0.0);
                            }
                            break;
                        case "Angry":
                            var angry = (string)row[column];
                            double angryparse;
                            success = double.TryParse(angry, out angryparse); 
                            if(success){
                                totalAngry.Add(Convert.ToDouble(angryparse));
                            }else{
                                totalAngry.Add(0.0);
                            }
                            break;
                        case "Surprised":
                            var surprised = (string)row[column];
                            double surprisedparse;
                            success = double.TryParse(surprised, out surprisedparse); 
                            if(success){
                                totalSurprised.Add(Convert.ToDouble(surprisedparse));
                            }else{
                                totalSurprised.Add(0.0);
                            }
                            break;
                        case "Scared":
                            var scared =(string) row[column];
                            double scaredparse;
                            success = double.TryParse(scared, out scaredparse); 
                            if(success){
                                totalScared.Add(Convert.ToDouble(scaredparse));
                            }else{
                                totalScared.Add(0.0);
                            }
                            break;
                        case "Disgusted":
                            var disgisted = (string)row[column];
                            double disgistedparse;
                            success = double.TryParse(disgisted, out disgistedparse); 
                            if(success){
                                totalDisgusted.Add(Convert.ToDouble(disgistedparse));
                            }else{
                                totalDisgusted.Add(0.0);
                            }                            
                            break;
                        case "Contempt":
                            var contempt = (string)row[column];
                            double contemptparse;
                            success = double.TryParse(contempt, out contemptparse); 
                            if(success){
                                totalContempt.Add(Convert.ToDouble(contemptparse));
                            }else{
                                totalContempt.Add(0.0);
                            }
                            break;
                    }
                }
            }

            processedResult.FaceReader.FaceReaderDatas = faceReaderDatas;
            context.Results.Update(processedResult);
            context.SaveChanges();
        }

    }
}



