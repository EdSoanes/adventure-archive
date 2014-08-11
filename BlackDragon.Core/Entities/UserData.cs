using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackDragon.Core.Entities
{
	public class UserData : IdentifiableItem
	{
        public List<UserDataWorld> Worlds { get; set; }
        public UserDataWorld SelectedWorld { get; set; }
        public List<UserDataWorld> FavouriteWorlds { get; set; }

        public UserDataAdventure SelectedAdventure { get; set; }

        public List<Note> Notes { get; set; }

        public UserData()
        {
            Worlds = new List<UserDataWorld>();
            FavouriteWorlds = new List<UserDataWorld>();
            Notes = new List<Note>();
        }

        //public List<Note> Notes
        //{
        //    get;
        //    set;
        //}

        //public Page CurrentPage
        //{
        //    get;
        //    set;
        //}

        //public List<UserDataArchiveAction> CurrentActions = new List<UserDataArchiveAction>();
		
        //public void AddAction(ContentTypes type, ArchiveAction action)
        //{
        //    var existing = CurrentActions.FirstOrDefault(x => x.ContentType == type);
        //    if (existing != null)
        //        CurrentActions.Remove(existing);

        //    CurrentActions.Add(new UserDataArchiveAction { ContentType = type, Action = action });
        //}
		
        //public ArchiveAction GetAction(ContentTypes type)
        //{
        //    var existing = CurrentActions.FirstOrDefault(x => x.ContentType == type);
        //    return existing != null ? existing.Action : null;
        //}

        //public List<UserDataScreenLocation> ScreenLocations = new List<UserDataScreenLocation>();

        //public void AddScreenLocation(string pageContentPath, string contentPath, UserDataContent screenLocation)
        //{
        //    var existing = ScreenLocations.FirstOrDefault(x => x.PageContentPath == pageContentPath && x.ContentPath == contentPath);
        //    if (existing != null)
        //        ScreenLocations.Remove(existing);

        //    var udScreenLocation = new UserDataScreenLocation { PageContentPath = pageContentPath, ContentPath = contentPath, ScreenLocation = screenLocation };
        //    ScreenLocations.Add(udScreenLocation);
        //}

        //public UserDataContent GetScreenLocation(string pageContentPath, string contentPath)
        //{
        //    var existing = ScreenLocations.FirstOrDefault(x => x.PageContentPath == pageContentPath && x.ContentPath == contentPath);
        //    return existing != null ? existing.ScreenLocation : null;
        //}
	}
}

