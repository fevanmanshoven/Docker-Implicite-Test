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

namespace ImpliciteTesterServer.Pages.Results
{
    public partial class DetailsResults
    {
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IWebHostEnvironment env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Parameter] public string ResultId { get; set; }

        private Result result = new("");
        private Test test = new("");

        private MudForm detailForm;

        private IEnumerable<String> selectedPosEmotions { get; set; } = new HashSet<String>() { "Neutral", "Happy" };
        private IEnumerable<String> selectedNegEmotions { get; set; } = new HashSet<String>() { "Sad", "Angry", "Surprised", "Scared", "Disgusted", "Contempt" };

        private string posCategories = "";
        private string negCategories = "";


        private MyDbContext context = new MyDbContext();

        private DateTime startTime;
        private DateTime endTime;

        private List<String> timeLineResult = new();
        private bool detailedOverview = false;

        List<string> emotions = new List<string> { "Neutral","Happy", "Sad", "Angry", "Surprised", "Scared", "Disgusted", "Contempt"};

        List<double> postiveAllImagesData = new();
        List<string> postiveAllImagesLabels = new();

        List<double> negativeAllImagesData = new();
        List<string> negativeAllImagesLabels = new();

        List<double> positiveImageEmotionsData = new();
        List<string> positiveImageEmotionsLabels = new();

        List<double> negativeImageEmotionsData = new();
        List<string> negativeImageEmotionsLabels = new();

        List<double> postiveStrictImagesData = new();
        List<string> postiveStrictImagesLabels = new();

        List<double> negativeStrictImagesData = new();
        List<string> negativeStrictImagesLabels = new();




        bool loading = true;
        bool loadingUpload = false;
        bool startedcalculating = false;
        bool finishedcalculating = false;

        bool success;
        string[] errors = { };

        protected override async Task OnInitializedAsync()
        {
            getResult(Int16.Parse(ResultId));
            loading = false;
        }

        protected void getResult(int resultId)
        {
            Snackbar.Add("Result loading...");
            result = context.Results.Where(r => r.ResultId == resultId)
                .Include(r => r.Test).Include(r => r.FaceReader).ThenInclude(f => f.FaceReaderDatas).FirstOrDefault();
            test = context.Tests.Where(t => t.TestId == result.Test.TestId).Include(x => x.PosCategories).Include(x => x.NegCategories).Include(x => x.Fases).ThenInclude(x => x.FaseTypeImages).ThenInclude(f => f.Image).FirstOrDefault();

            posCategories = (string.Join(", ", test.PosCategories.Select(f => f.Name)));
            negCategories = (string.Join(", ", test.NegCategories.Select(f => f.Name)));

            Snackbar.Add("Result loaded...");
        }

        private void setTimeLineResults()
        {
            List<string> postiveEmotions = new List<string>();
            List<string> neagtiveEmotions = new List<string>();

            var totalSecondsReal = (startTime - endTime).TotalSeconds;
            var totalSeconds = 1;
            var faseCounter = 0;
            List<string> timeLine = new();
            timeLine.Add("  ");
            timeLine.Add("Calculation overview");
            timeLine.Add("_______________________________");
            foreach (Fase fase in test.Fases)
            {
                timeLine.Add("Fase " + faseCounter);
                timeLine.Add("----------");
                foreach (FaseTypeImage faseTypeImage in fase.FaseTypeImages)
                {
                    for (int i = totalSeconds; i < totalSeconds + fase.Duration; i++)
                    {
                        FaceReaderData faceReaderData = result.FaceReader.FaceReaderDatas.Where(fd => fd.Time == i).FirstOrDefault();
                        timeLine.Add("Second " + i + ": " + faseTypeImage.FaseType.ToString() + " image");
                        if(faceReaderData != null)
                        {
                            if (detailedOverview)
                            {
                                foreach (var (key, value) in faceReaderData.getEmotionData())
                                {
                                    timeLine.Add(key + " : " + value);
                                }
                            }
                            timeLine.Add("Dominant emotion: " + faceReaderData.getStrongEmotionName(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()) + " - " + faceReaderData.isPositiveEmotion(selectedPosEmotions.ToList(),selectedNegEmotions.ToList()));
                        }
                        timeLine.Add("-");
                    }
                    totalSeconds += fase.Duration;
                }
                faseCounter += 1;
            }
            timeLineResult = timeLine;
            Snackbar.Add("Timeline loaded");
        }

        private void calculateResults()
        {
            startedcalculating = true;
            StateHasChanged();
            Snackbar.Add("Calculating started");
            resetCharts();
            setTimeLineResults();
            setChartImageEmotions();
            finishedcalculating = true;
            StateHasChanged();
        }

        private void resetCharts()
        {
            postiveAllImagesData = new();
            postiveAllImagesLabels = new();

            negativeAllImagesData = new();
            negativeAllImagesLabels = new();

            positiveImageEmotionsData = new();
            positiveImageEmotionsLabels = new();

            negativeImageEmotionsData = new();
            negativeImageEmotionsLabels = new();

            postiveStrictImagesData = new();
            postiveStrictImagesLabels = new();

            negativeStrictImagesData = new();
            negativeStrictImagesLabels = new();
        }

        private void setChartImageEmotions()
        {
            Dictionary<string, List<double>> postiveAllImages = new Dictionary<string, List<double>>() { { "Neutral", new() {0.0} }, { "Happy", new() { 0.0 } }, { "Sad", new() { 0.0 } }, { "Angry", new() { 0.0 } }, { "Surprised", new() { 0.0 } }, { "Scared", new() { 0.0 } }, { "Disgusted", new() { 0.0 } }, { "Contempt", new() { 0.0 } } };
            Dictionary<string, List<double>> negativeAllImages = new Dictionary<string, List<double>>() { { "Neutral", new() { 0.0 } }, { "Happy", new() { 0.0 } }, { "Sad", new() { 0.0 } }, { "Angry", new() { 0.0 } }, { "Surprised", new() { 0.0 } }, { "Scared", new() { 0.0 } }, { "Disgusted", new() { 0.0 } }, { "Contempt", new() { 0.0 } } };

            Dictionary<string, List<double>> postiveStrongImages = new Dictionary<string, List<double>>() { { "Neutral", new() { 0.0 } }, { "Happy", new() { 0.0 } }, { "Sad", new() { 0.0 } }, { "Angry", new() { 0.0 } }, { "Surprised", new() { 0.0 } }, { "Scared", new() { 0.0 } }, { "Disgusted", new() { 0.0 } }, { "Contempt", new() { 0.0 } } };
            Dictionary<string, List<double>> negativeStrongImages = new Dictionary<string, List<double>>() { { "Neutral", new() { 0.0 } }, { "Happy", new() { 0.0 } }, { "Sad", new() { 0.0 } }, { "Angry", new() { 0.0 } }, { "Surprised", new() { 0.0 } }, { "Scared", new() { 0.0 } }, { "Disgusted", new() { 0.0 } }, { "Contempt", new() { 0.0 } } };

            Dictionary<string, List<double>> postiveStrictImages = new Dictionary<string, List<double>>() { };
            Dictionary<string, List<double>> negativeStrictImages = new Dictionary<string, List<double>>() { };



            var totalSeconds = 1;
            foreach (Fase fase in test.Fases)
            {
                foreach (FaseTypeImage faseTypeImage in fase.FaseTypeImages)
                {
                    for (int i = totalSeconds; i < totalSeconds + fase.Duration; i++)
                    {
                        FaceReaderData faceReaderData = result.FaceReader.FaceReaderDatas.Where(fd => fd.Time == i).FirstOrDefault();
                        if (faceReaderData != null)
                        {
                            if (faseTypeImage.FaseType.Equals(FaseType.Positive))
                            {
                                foreach (var (key, value) in faceReaderData.getAllEmotions(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()))
                                {
                                    addOrUpdate(postiveAllImages, key,value);
                                }
                                addOrUpdate(postiveStrongImages, faceReaderData.getStrongEmotionName(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()), faceReaderData.getStrongEmotionValue(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()));
                                addOrUpdate(postiveStrictImages, faceReaderData.isPositiveEmotion(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()), 1);

                            }
                            else if (faseTypeImage.FaseType.Equals(FaseType.Negative))
                            {
                                foreach (var (key, value) in faceReaderData.getAllEmotions(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()))
                                {
                                    addOrUpdate(negativeAllImages, key, value);
                                }
                                addOrUpdate(negativeStrongImages, faceReaderData.getStrongEmotionName(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()), faceReaderData.getStrongEmotionValue(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()));
                                addOrUpdate(negativeStrictImages, faceReaderData.isPositiveEmotion(selectedPosEmotions.ToList(), selectedNegEmotions.ToList()), 1);
                            }
                        }
                    }
                    totalSeconds += fase.Duration;
                }
            }
            foreach (var (key, value) in postiveAllImages)
            {
                postiveAllImagesLabels.Add(key);
                postiveAllImagesData.Add(value.Average());
            }
            foreach (var (key, value) in negativeAllImages)
            {
                negativeAllImagesLabels.Add(key);
                negativeAllImagesData.Add(value.Average());
            }

            foreach (var (key, value) in postiveStrongImages)
            {
                positiveImageEmotionsLabels.Add(key);
                positiveImageEmotionsData.Add(value.Average());
            }
            foreach (var (key, value) in negativeStrongImages)
            {
                negativeImageEmotionsLabels.Add(key);
                negativeImageEmotionsData.Add(value.Average());
            }

            foreach (var (key, value) in postiveStrictImages)
            {
                postiveStrictImagesLabels.Add(key);
                postiveStrictImagesData.Add(value.Sum());
            }
            foreach (var (key, value) in negativeStrictImages)
            {
                negativeStrictImagesLabels.Add(key);
                negativeStrictImagesData.Add(value.Sum());
            }
            Snackbar.Add("Chart loaded");
        }

        void addOrUpdate(Dictionary<String, List<double>> dic, string key, double newValue)
        {
            List<double> val;
            if (dic.TryGetValue(key, out val))
            {
                // yay, value exists!
                val.Add(newValue);
                dic[key] = val;
            }
            else
            {
                // darn, lets add the value
                List<double> newList = new() { newValue };
                dic.Add(key, newList);
            }
        }

        private async Task Remove(Result result)
        {
            result = new Result("");
            context.Results.Remove(result);
            await context.SaveChangesAsync();
        }

    }

}



