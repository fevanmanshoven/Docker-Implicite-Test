using System;
namespace ImpliciteTesterServer.Data
{
	public class Category
	{

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public List<ImageUpload> ImageUploads { get; set; }


        public Category()
		{
		}

        public override string ToString() {
            return Name;
        }

    }
}

