using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using BlackDragon.Core.Entities;
using Newtonsoft.Json;

namespace BlackDragon.Core
{
    public class AdventureService : IAdventureService
    {
//        private readonly IWorldService _worldService;
//        private readonly IFileSource _fileSource;

//        public event EventHandler AdventureSelected;

//        private Page _currentPage = null;

//        private Adventure _adventure;
//        public Adventure Adventure
//        {
//            get { return _adventure; }
//            private set
//            {
//                if (_adventure != value)
//                {
//                    _adventure = value;
//                    Console.WriteLine("Setting adventure");
//                    if (AdventureSelected != null)
//                    {
//                        Console.WriteLine("Calling AdventureSelected event handler");
//                        AdventureSelected.Invoke(this, new EventArgs());
//                    }
//                }
//            }
//        }

//        public AdventureService(IWorldService worldService, IFileSource fileSource)
//        {
//            _worldService = worldService;
//            _fileSource = fileSource;
//        }

//        public void Load(string adventureBaseUrl)
//        {
//            //Load the adventure
//            adventureBaseUrl = _worldService.Domain.CombineUrl(adventureBaseUrl);
//            Console.WriteLine("Loading adventure: " + adventureBaseUrl);

//            var adventureJson = _fileSource.GetTextFile(adventureBaseUrl.CombineUrl("json"));
//            var adventure = JsonConvert.DeserializeObject<Adventure>(adventureJson);

//            //Load user data
////            var safeFileName = (adventureBaseUrl + "_userdata").SafeFileName(".json");
////            var localStorageFileName = Path.Combine(Settings.LocalStoragePath, safeFileName);

//            _currentPage = adventure.Chapters[0].Pages[0];

//            //if (File.Exists(localStorageFileName))
//            //{
//            //    using(var file = File.OpenText(localStorageFileName))
//            //    {
//            //        var json = file.ReadToEnd();
//            //        UserData = JsonConvert.DeserializeObject<UserData>(json);
//            //        foreach (var page in adventure.GetAllPages())
//            //        {
//            //            var screenLocation = UserData.GetScreenLocation(page.ContentPath, page.ContentPath);
//            //            if (screenLocation != null)
//            //                page.ContentUserData = screenLocation;

//            //            if (page.Map != null)
//            //            {
//            //                var mapScreenLocation = UserData.GetScreenLocation(page.ContentPath, page.Map.ContentPath);
//            //                if (mapScreenLocation != null)
//            //                    page.Map.ContentUserData = mapScreenLocation;
//            //            }
//            //        }
//            //    }
//            //}
//            //else
//            //{
//            //    Console.WriteLine("File '" + localStorageFileName + "' does not exist");
//            //    UserData = new UserData();
//            //}

//            //if (UserData.CurrentPage == null)
//            //    UserData.CurrentPage = adventure.GetAllPages().First();

//            Adventure = adventure;
//        }

//        public void Save()
//        {
//            //if (Adventure != null)
//            //{
//            //    foreach (var page in Adventure.GetAllPages())
//            //    {
//            //        if (page.ContentUserData.HasLocation)
//            //            UserData.AddScreenLocation(page.ContentPath, page.ContentPath, page.ContentUserData);

//            //        if (page.Map != null && page.Map.ContentUserData.HasLocation)
//            //            UserData.AddScreenLocation(page.ContentPath, page.Map.ContentPath, page.Map.ContentUserData);
//            //    }

//            //    var adventureBaseUrl = Hub.WorldService.Domain.CombineUrl(Adventure.ContentPath);
//            //    var safeFileName = (adventureBaseUrl + "_userdata").SafeFileName(".json");
//            //    var localStorageFileName = Path.Combine(Settings.LocalStoragePath, safeFileName);

//            //    using (var file = File.CreateText(localStorageFileName))
//            //    {
//            //        var text = JsonConvert.SerializeObject(UserData);
//            //        file.Write(text);
//            //    }
//            //}
//        }

//        #region Actions

//        private Dictionary<ContentTypes, ArchiveAction> _currentActions = new Dictionary<ContentTypes, ArchiveAction>();
//        public ArchiveAction GetCurrentAction(ContentTypes type)
//        {
//            if (_currentActions.ContainsKey(type))
//                return _currentActions[type];
//            else
//                return null;
//        }

//        public void SetCurrentAction(ContentTypes type, ArchiveAction action)
//        {
//            if (_currentActions.ContainsKey(type))
//                _currentActions.Remove(type);

//            _currentActions.Add(type, action);
//        }

//        #endregion Actions

//        #region Page Navigation

//        public List<Page> AllPages()
//        {
//            if (Adventure != null)
//                return Adventure.GetAllPages();
//            else
//                return new List<Page>();
//        }

//        public Page CurrentPage()
//        {
//            return _currentPage;
//        }

//        public Chapter ChapterForPage(Page page)
//        {
//            if (Adventure != null)
//                return Adventure.GetChapterForPage(page);
//            else
//                return null;
//        }

//        public Page PrevPage()
//        {
//            var curr = CurrentPage();
//            if (curr != null)
//            {
//                var prev = Adventure.GetPageBefore(curr);
//                _currentPage = prev;
//                return prev;
//            }

//            return null;
//        }

//        public Page NextPage()
//        {
//            var curr = CurrentPage();
//            if (curr != null)
//            {
//                var next = Adventure.GetPageAfter(curr);
//                _currentPage = next;
//                return next;
//            }

//            return null;
//        }

//        #endregion Page Navigation
    }
}

