using System;
using Microsoft.AspNetCore.Components.Forms;

namespace ImpliciteTesterServer.Data
{
	public class ImageUpload
	{
        public int ImageUploadId { get; set; }

        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public List<Image> Images { get; set; }

        public List<Category> Categories { get; set; }

        public ImageUpload()
		{
		}

        public override string ToString()
        {
            return Name;
        }
    }
}

