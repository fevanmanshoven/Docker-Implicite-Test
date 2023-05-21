using System;
namespace ImpliciteTesterServer.Data
{
	public class Result
	{

        public int ResultId { get; set; }

        public string Name { get; set; }

        public string Participant { get; set; }

        public Test Test { get; set; }

        public string[] TimeLineResult { get; set; }

        public FaceReader FaceReader { get; set; }

        public Result(string name)
        {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

    }
}

