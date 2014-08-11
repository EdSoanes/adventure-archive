using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Core.Entities
{
	public class UserDataAdventure : IdentifiableItem
    {
        public string Url
        {
            get;
            set;
        }

        public string AbsoluteImageUrl
        {
            get { return string.IsNullOrEmpty(ImageUrl) ? string.Empty : Url.UrlDomain() + ImageUrl; }
        }

        public DateTime? LastAccessed
        {
            get;
            set;
        }

        public void SetFromAdventure(Adventure adventure)
        {
            Title = adventure.Title;
            Subtitle = adventure.Subtitle;
            Url = adventure.ContentPath;
            ImageUrl = adventure.ImageUrl;
        }
    }
}