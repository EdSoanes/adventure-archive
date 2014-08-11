using BlackDragon.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackDragon.Core
{
    public class UserDataContentService : IUserDataContentService
    {
		private readonly IFileAccessService _fileService;

        private Dictionary<string, UserDataContent> _userData = new Dictionary<string, UserDataContent>();

        private string _userDataLocalFileName = "";
        private string _userDataWorldListFileName = "";

        private List<UserDataWorld> _userDataWorlds = new List<UserDataWorld>();
        public List<UserDataWorld> WorldData
        {
            get { return _userDataWorlds; }
        }

		public UserDataContentService(IFileAccessService fileService)
        {
            _fileService = fileService;
            _userDataWorldListFileName = Path.Combine(Settings.LocalStoragePath, "worlddata.json");
        }

        public void LoadWorlds()
        {
			var res = _fileService.DeserializeFromLocalStorage<UserDataWorldList>(_userDataWorldListFileName);
            if (res != null)
                _userDataWorlds = res.WorldData;

            if (!_userDataWorlds.Any(x => x.Title == "Test 1"))
                _userDataWorlds.Add(new UserDataWorld { Title = "Test 1", Subtitle = "Test world 1 for testing purposes. Does not work", Url = "http://nowhere1.com" });

            if (!_userDataWorlds.Any(x => x.Title == "Test 2"))
                _userDataWorlds.Add(new UserDataWorld { Title = "Test 2", Subtitle = "Test world 2 for testing purposes. Does not work", Url = "http://nowhere2.com" });
        }

        public void SaveWorlds()
        {
            if (_userDataWorlds != null)
            {
                var data = new UserDataWorldList();
                data.WorldData = _userDataWorlds;
				_fileService.SerializeToLocalStorage(_userDataWorldListFileName, data);
            }
        }

        public void AddWorld(World world)
        {
            if (world != null)
            {
                var userDataWorld = _userDataWorlds.FirstOrDefault(x => x.Url.ToLower() == world.ContentPath.ToLower());
                if (userDataWorld == null)
                {
                    userDataWorld = new UserDataWorld();
                    _userDataWorlds.Insert(0, userDataWorld);
                }

                userDataWorld.SetFromWorld(world);
                SaveWorlds();
            }
        }

        public void DeleteWorld(UserDataWorld userDataWorld)
        {
            if (userDataWorld != null && _userDataWorlds.Contains(userDataWorld))
            {
                _userDataWorlds.Remove(userDataWorld);
                SaveWorlds();
            }
        }

        public void LoadWorldUserData(string worldBasePath)
        {
            _userData.Clear();

            _userDataLocalFileName = Path.Combine(Settings.LocalStoragePath, worldBasePath.SafeFileName("userdata.json"));
			var res = _fileService.DeserializeFromLocalStorage<IEnumerable<UserDataContent>>(_userDataLocalFileName);

            if (res != null)
            {
                foreach (var item in res)
                    _userData.Add(item.ContentPath.ToLower(), item);
            }
        }

        public void SaveWorldUserData()
        {
            if (_userData != null && !string.IsNullOrEmpty(_userDataLocalFileName))
            {
                var res = _userData.Values.ToArray();
				_fileService.SerializeToLocalStorage(_userDataLocalFileName, res);
            }
        }

        public UserDataContent GetWorldUserDataItem(string contentPath)
        {
            var key = contentPath.ToLower();

            if (_userData.ContainsKey(key))
                return _userData[key];
            else
                return null;
        }

        public void SetWorldUserDataItem(UserDataContent userDataContent)
        {
            var key = userDataContent.ContentPath.ToLower();

            if (_userData.ContainsKey(key))
                _userData.Remove(key);

            _userData.Add(key, userDataContent);

            SaveWorldUserData();
        }
    }
}
