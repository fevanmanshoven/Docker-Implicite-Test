using System;
using System.Collections.Specialized;

namespace DockerImpliciteTest.Data
{
	public class Fase
	{

        public int FaseId { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public FaseType FaseType { get; set; }

        public int Duration { get; set; }

        public int ImgAmount { get; set; }

        public List<FaseTypeImage> FaseTypeImages { get; set; }

        public Fase(string name)
        {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

    }
}

