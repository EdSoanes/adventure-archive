using System;
using Newtonsoft.Json;
using BlackDragon.Core;

namespace BlackDragon.Core.Entities
{
	public class IdentifiableItem : ICloneable
	{
		public IdentifiableItem()
		{
		}
		
		public bool IsNew
		{
			get { return string.IsNullOrEmpty(Identifier); }
		}
		
		public string Identifier
		{
			get;
			set;
		}

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

        public string ImageUrl
        {
            get;
            set;
        }

        private string _contentPath = "";
		public string ContentPath
		{
            get { return _contentPath; }
			set
            {
                if (value.ToLower().EndsWith(".aspx"))
                    _contentPath = value.Substring(0, value.Length - 5);
                else
                    _contentPath = value;
            }
		}

		[JsonIgnore]
		public string AbsoluteContentPath 
		{
			get;
			set;
		}

		public object Clone()
		{
			return OnClone();
		}
		
		protected virtual object OnClone()
		{
			return this.MemberwiseClone();
		}
	}
}

