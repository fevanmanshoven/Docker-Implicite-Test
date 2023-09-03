using System;
namespace DockerImpliciteTest.Data
{
	public class Category
	{

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public List<ImageUpload> ImageUploads { get; set; }

        public List<Test> PostCategorieTests { get; set; }

        public List<Test> NegCategorieTests { get; set; }
        

        public Category()
		{
		}

        public override string ToString() {
            return Name;
        }

    }
}

