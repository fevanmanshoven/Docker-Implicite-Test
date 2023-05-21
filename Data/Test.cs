using System;
namespace ImpliciteTesterServer.Data
{
	public class Test
	{

        public int TestId { get; set; }

        public string Name { get; set; }

        public List<ImageUpload> ?PosImageUploads { get; set; }

        public List<Category> ?PosCategories { get; set; }

        public List<ImageUpload>? NegImageUploads { get; set; }

        public List<Category>? NegCategories { get; set; }

        public List<Fase>? Fases { get; set; }

        public List<Result>? Results { get; set; }


        public Test(string name)
        {
            Name = name;
        }

        public override string ToString() {
            return Name;
        }

    }
}

