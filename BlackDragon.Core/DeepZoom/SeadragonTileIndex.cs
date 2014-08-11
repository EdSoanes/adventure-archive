using System;

namespace BlackDragon.Core.DeepZoom
{
    public class SeadragonTileIndex
    {
		public SeadragonTileIndex(int col, int row, int levelOfDetail)
        {
			Col = col;
			Row = row;
			LevelOfDetail = levelOfDetail;
        }

		public int Col { get; set; }
		public int Row { get; set; }
		public int LevelOfDetail { get; set; }

		public override string ToString()
		{
			return string.Format("[SeadragonTileIndex: Col={0}, Row={1}, LevelOfDetail={2}]", Col, Row, LevelOfDetail);
		}
    }
}

