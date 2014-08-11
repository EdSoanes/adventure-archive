using System;

namespace BlackDragon.Core.DeepZoom
{
    public class SeadragonOverlay
    {
		public string Title
		{
			get;
			set;
		}

		public string Subtitle
		{
			get;
			set;
		}

		public int X
		{
			get;
			set;
		}

		public int Y
		{
			get;
			set;
		}

		public int OriginX
		{
			get;
			set;
		}

		public int OriginY
		{
			get;
			set;
		}

		public bool IsSelected
		{
			get;
			set;
		}

		public object Image
		{
			get;
			set;
		}

		public object SelectedImage
		{
			get;
			set;
		}

		public int FirstVisibleOnLevelOfDetail
		{
			get;
			set;
		}

        public SeadragonOverlay()
        {
        }

		public override string ToString()
		{
			return string.Format("[SeadragonOverlay: Title={0}, Subtitle={1}, X={2}, Y={3}, OriginX={4}, OriginY={5}, IsSelected={6}, Image={7}, SelectedImage={8}, FirstVisibleOnLevelOfDetail={9}]", Title, Subtitle, X, Y, OriginX, OriginY, IsSelected, Image, SelectedImage, FirstVisibleOnLevelOfDetail);
		}
    }
}

