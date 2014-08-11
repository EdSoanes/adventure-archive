using System;

namespace BlackDragon.Core.DeepZoom
{
	public class SeadragonOverlaySelectedEventArgs : EventArgs
    {
		public SeadragonOverlay Overlay
		{
			get;
			private set;
		}

		public SeadragonOverlaySelectedEventArgs(SeadragonOverlay overlay)
        {
			Overlay = overlay;
        }
    }

	public class SeadragonTileDownloadedEventArgs : EventArgs
	{
		public SeadragonTileIndex Index
		{
			get;
			private set;
		}

		public SeadragonTileDownloadedEventArgs(SeadragonTileIndex index)
		{
			Index = index;
		}
	}
}

