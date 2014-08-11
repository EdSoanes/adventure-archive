using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Core.Entities
{
	public class UserDataWorld : IdentifiableItem
    {
        public string Url
        {
            get;
            set;
        }

        public string AbsoluteImageUrl
        {
			get { return string.IsNullOrEmpty(ImageUrl) ? string.Empty : Url.UrlDomain().CombineUrl(ImageUrl); }
        }

        public DateTime? LastAccessed
        {
            get;
            set;
        }

		public StatusCode? LastAccessedStatusCode
		{
			get;
			set;
		}

        public void SetFromWorld(World world)
        {
            Title = world.Title;
            Subtitle = world.Subtitle;
			ContentPath = world.ContentPath;
            ImageUrl = world.ImageUrl;
			AbsoluteContentPath = Url.UrlDomain().CombineUrl(ContentPath);
        }
    }
}
