using System;
namespace ImpliciteTesterServer.Data
{
	public class FaseTypeImage
	{

        public int FaseTypeImageId { get; set; }

        public FaseType FaseType { get; set; }

        public Image Image { get; set; }

        public Fase Fase { get; set; }

        public FaseTypeImage()
        {
        }

        public FaseTypeImage(Image img, FaseType faseType)
        {
            Image = img;
            FaseType = faseType;
	    }
}
}

