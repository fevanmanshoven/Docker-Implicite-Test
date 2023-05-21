using System;
using Microsoft.AspNetCore.Components.Forms;

namespace ImpliciteTesterServer.Data
{
	public class FaceReaderData
	{
        public int FaceReaderDataId { get; set; }

        public int Time { get; set; }

        public double Neutral { get; set; }

        public double Happy { get; set; }

        public double Sad { get; set; }

        public double Angry { get; set; }

        public double Surprised { get; set; }

        public double Scared { get; set; }

        public double Disgusted { get; set; }

        public double Contempt { get; set; }



        public FaceReaderData(int time, double neutral, double happy, double sad, double angry, double surprised, double scared, double disgusted, double contempt)
        {
            Time = time;
            Neutral = neutral;
            Happy = happy;
            Sad = sad;
            Angry = angry;
            Surprised = surprised;
            Scared = scared;
            Disgusted = disgusted;
            Contempt = contempt;
        }

        public Dictionary<string, double> getEmotionData()
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>();
            emotions.Add("Neutral", Neutral);
            emotions.Add("Happy", Happy);
            emotions.Add("Sad", Sad);
            emotions.Add("Angry", Angry);
            emotions.Add("Surprised", Surprised);
            emotions.Add("Scared", Scared);
            emotions.Add("Disgusted", Disgusted);
            emotions.Add("Contempt", Contempt);
            return emotions;
        }

        public Dictionary<string, double> getAllEmotions(List<string> postiveEmotions, List<string> neagtiveEmotions)
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>();
            emotions.Add("Neutral", Neutral);
            emotions.Add("Happy", Happy);
            emotions.Add("Sad", Sad);
            emotions.Add("Angry", Angry);
            emotions.Add("Surprised", Surprised);
            emotions.Add("Scared", Scared);
            emotions.Add("Disgusted", Disgusted);
            emotions.Add("Contempt", Contempt);
            foreach (var (key, value) in emotions)
            {
                if (!postiveEmotions.Contains(key) && !neagtiveEmotions.Contains(key))
                {
                    emotions.Remove(key);
                }
            }
            return emotions;
        }

        public string getStrongEmotionName(List<string> postiveEmotions, List<string> neagtiveEmotions)
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>();
            emotions.Add("Neutral", Neutral);
            emotions.Add("Happy", Happy);
            emotions.Add("Sad", Sad);
            emotions.Add("Angry", Angry);
            emotions.Add("Surprised", Surprised);
            emotions.Add("Scared", Scared);
            emotions.Add("Disgusted", Disgusted);
            emotions.Add("Contempt", Contempt);
            double strongEmotionValue = 0;
            string strongEmotionKey = "";
            foreach (var (key, value) in emotions)
            {
                if (postiveEmotions.Contains(key) || neagtiveEmotions.Contains(key))
                {
                    if (value > strongEmotionValue)
                    {
                        strongEmotionKey = key;
                        strongEmotionValue = value;
                    }
                }
            }
            return strongEmotionKey;
        }

        public double getStrongEmotionValue(List<string> postiveEmotions, List<string> negativeEmotions )
        {
            Dictionary<string, double> emotions = new Dictionary<string, double>();
            emotions.Add("Neutral", Neutral);
            emotions.Add("Happy", Happy);
            emotions.Add("Sad", Sad);
            emotions.Add("Angry", Angry);
            emotions.Add("Surprised", Surprised);
            emotions.Add("Scared", Scared);
            emotions.Add("Disgusted", Disgusted);
            emotions.Add("Contempt", Contempt);
            double strongEmotionValue = 0;
            string strongEmotionKey = "";
            foreach (var (key, value) in emotions)
            {
                if(postiveEmotions.Contains(key) || negativeEmotions.Contains(key))
                {
                    if (value > strongEmotionValue)
                    {
                        strongEmotionKey = key;
                        strongEmotionValue = value;
                    }
                }

            }
            return strongEmotionValue;
        }

        public string isPositiveEmotion(List<string> postiveEmotions, List<string> negativeEmotions)
        {
            string strongEmotion = getStrongEmotionName(postiveEmotions, negativeEmotions);
            if (postiveEmotions.Contains(strongEmotion))
            {
                return "Positive";
            }
            else if(negativeEmotions.Contains(strongEmotion))
            {
                return "Negative";
            }
            else
            {
                return "Not defined";
            }
        }

        //public override string ToString()
        //{
        //    return Name;
        //}
    }
}

