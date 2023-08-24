using System;
using Microsoft.AspNetCore.Components.Forms;

namespace DockerImpliciteTest.Data
{
	public class FaceReader
	{
        public int FaceReaderId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Source { get; set; }

        public List<FaceReaderData> FaceReaderDatas { get; set; }

        public FaceReader()
        {

        }

        public FaceReader(string name, string source, string path)
		{
            Name = name;
            Source = source;
            Path = path;
		}

        public override string ToString()
        {
            return Name;
        }
    }
}

