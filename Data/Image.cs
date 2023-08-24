using System;
using Microsoft.AspNetCore.Components.Forms;

namespace DockerImpliciteTest.Data
{
	public class Image
	{
        public int ImageId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Source { get; set; }

        public ImageUpload ImageUpload { get; set; }

        public Image()
        {

        }

        public Image(string name, string source, string path, ImageUpload imageUpload)
		{
			Name = name;
            Source = source;
            Path = path;
            ImageUpload = imageUpload;
		}

        public override string ToString()
        {
            return Name;
        }
    }
}

