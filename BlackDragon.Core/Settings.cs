using System;

namespace BlackDragon.Core
{
    public class Settings
    {
        public const string Protocol = "http://";

        public static string LocalStoragePath
        {
            get { return Environment.GetFolderPath (Environment.SpecialFolder.Personal); }
        }

        public static bool IsRetina
        {
            get;
            set;
        }

        public static float GetZoomFactor(float zoomScale)
        {
            var s = Settings.IsRetina ? zoomScale / 2 : zoomScale;
            if (s <= 0.185)
                s = 0.125f;
            else if (s <= 0.38)
                s = 0.25f;
            else if (s <= 0.75)
                s = 0.5f;
            else
                s = 1f;

            return s;
        }

        public static int GetLevelOfDetail(float zoomScale)
        {
            var s = zoomScale;//Settings.IsRetina ? zoomScale / 2 : zoomScale;
            if (s <= 0.185)
                return 1;
            else if (s <= 0.38)
                return 2;
            else if (s <= 0.75)
                return 3;
            else
                return 4;
        }

        public static float LevelOfDetailScale(int levelOfDetail)
        {
            switch (levelOfDetail)
            {
                case 1: return 0.125f;
                case 2: return 0.25f;
                case 3: return 0.5f;
                default: return 1f;
            }
        }
    }
}

