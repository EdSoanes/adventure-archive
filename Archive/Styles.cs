using System;
using MonoTouch.UIKit;

namespace BlackDragon.Archive
{
    public class Styles
    {
		private static UIFont _fontTitle18;
		public static UIFont FontTitle18
		{
			get
			{
				if (_fontTitle18 == null)
					_fontTitle18 = UIFont.FromName("LaoMN", 18);

				return _fontTitle18;
			}
		}

		private static UIFont _fontSubtitle11;
		public static UIFont FontSubtitle11
		{
			get
			{
				if (_fontSubtitle11 == null)
					_fontSubtitle11 = UIFont.FromName("LaoMN", 11);

				return _fontSubtitle11;
			}
		}

		private static UIColor _titleLight;
		public static UIColor TitleLight
		{
			get
			{
				if (_titleLight == null)
					_titleLight = UIColorFromRgb(0xfe, 0xcc, 0x66);

				return _titleLight;
			}
		}

		private static UIColor UIColorFromRgb(float r, float g, float b, float alpha = 1f)
		{
			return new UIColor(r / 255.0f, g / 255.0f, b / 255.0f, alpha);
		}
    }
}

