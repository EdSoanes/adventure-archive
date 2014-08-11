using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using BlackDragon.Core;

namespace BlackDragon.Core.DeepZoom
{
    public class SeadragonFileDescriptor
    {
		public string Url { get; set; }
        public int TileSize { get; private set; }
        public int Overlap { get; private set; }
        public string Format { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Levels { get; private set; }

        public int TilesX { get; set; }
        public int TilesY { get; set; }

        public SeadragonFileDescriptor()
        {
        }

        public SeadragonFileDescriptor(string xml)
        {
            FromXml(xml);
        }

        public bool FromXml(string xml)
        {
            //string file = @"C:\projects\TheArchive\blackdragonia\dzc_output_images\island map.xml";
            //var xml = File.ReadAllText(file);
			XDocument doc = XDocument.Parse(GetTextWithoutBOM(xml), LoadOptions.None);
            var sizeNode = doc.Root.Descendants().First(x => x.Name.LocalName == "Size");

            TileSize = int.Parse(doc.Root.Attribute("TileSize").Value);
            Overlap = int.Parse(doc.Root.Attribute("Overlap").Value);
            Format = doc.Root.Attribute("Format").Value;
            Width = int.Parse(sizeNode.Attribute("Width").Value);
            Height = int.Parse(sizeNode.Attribute("Height").Value);
            Levels = (int)Math.Ceiling(Math.Log(Math.Max(Width, Height), 2));
            TilesX = Convert.ToInt32(Math.Ceiling((float)Width / TileSize));
            TilesY = Convert.ToInt32(Math.Ceiling((float)Height / TileSize));

            return true;
        }

		private string GetTextWithoutBOM(string textAsset)
		{
			string result = string.Empty;
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(textAsset)))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream, true))
				{
					result = streamReader.ReadToEnd();
				}
			}

			return result;
		}

        public int NoOfTilesX(int levelOfDetail)
        {
			var i = Levels - levelOfDetail;
            var res = Math.Ceiling(TilesX * (1 / Math.Pow(2, i)));
            
            return Convert.ToInt32(res);
        }

        public int NoOfTilesY(int levelOfDetail)
        {
			var i = Levels - levelOfDetail;
            var res = Math.Ceiling(TilesY * (1 / Math.Pow(2, i)));

            return Convert.ToInt32(res);
        }
    }
}
