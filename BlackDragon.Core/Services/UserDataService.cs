using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlackDragon.Core.Entities;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlackDragon.Core
{
    public class UserDataService : IUserDataService
    {
        private const string UserDataFileName = "UserData.json";

		private IFileAccessService _fileService;

        private UserData _userData;
        public UserData UserData
        {
            get { return _userData; }
        }

		public UserDataService(IFileAccessService fileSource)
        {
			_fileService = fileSource;
        }

		public Action LoadAction(Action<StatusCode> completeAction = null)
        {
            return () =>
            {
				StatusCode res = StatusCode.Ok;

				try
				{
	                if (File.Exists(Path.Combine(Settings.LocalStoragePath, UserDataFileName)))
					{
						_userData = _fileService.DeserializeFromLocalStorage<UserData>(Path.Combine(Settings.LocalStoragePath, UserDataFileName));
					}
	                else
	                {
	                    _userData = new UserData();
						TEMP_AddTestData();
	                }
				}
				catch (Exception ex)
				{
					res = ExceptionHandler.LoadFile(ex);
				}
				finally
				{
	                if (completeAction != null)
						completeAction.Invoke(res);
				}
            };
        }

		public Action LoadNewWorldAction(string url, Action completeAction = null)
		{
			if (!_userData.Worlds.Any(x => x.Url.ToLower() == url.ToLower()))
			{
				//Create a new user data world object and add it to the list
				var userDataWorld = new UserDataWorld { Url = url };
				_userData.Worlds.Insert(0, userDataWorld);

				return () =>
				{
					try
					{

						//Create the Uri of the new world to be loaded
						Uri uri = new Uri(url.AbsoluteHttpUrl().WithHttpProtocol());
						if (uri.Segments.Last().ToLower() != "json")
							uri = new Uri(uri, "json");

						var tcs = new TaskCompletionSource<FileCacheEntry>();
						_fileService.Request(uri.OriginalString, (fileCacheEntry) =>
						{
							tcs.SetResult(fileCacheEntry);
						});

						Task.WaitAll(new Task[] { tcs.Task });
						var json = tcs.Task.Result.GetData<string>();
						var world = JsonConvert.DeserializeObject<World>(json);

						if (world != null)
						{
							userDataWorld.Url = url;
							userDataWorld.SetFromWorld(world);
							userDataWorld.LastAccessed = DateTime.Now;
							userDataWorld.LastAccessedStatusCode = StatusCode.Ok;
						}
						else
							userDataWorld.LastAccessedStatusCode = StatusCode.JsonDeserializeFailed;
					}
					catch (Exception ex)
					{
						userDataWorld.LastAccessedStatusCode = ExceptionHandler.HttpDownload(ex);
					}
					finally
					{
						if (completeAction != null)
							completeAction.Invoke();
					}
				};
			}
			else
			{
				return null;
			}
		}

        public void Load()
        {
            if (File.Exists(Path.Combine(Settings.LocalStoragePath, UserDataFileName)))
				_userData = _fileService.DeserializeFromLocalStorage<UserData>(Path.Combine(Settings.LocalStoragePath, UserDataFileName));
            else
            {
                _userData = new UserData();
				TEMP_AddTestData();
            }
        }

        public void Save()
        {
			_fileService.SerializeToLocalStorage(Path.Combine(Settings.LocalStoragePath, UserDataFileName), _userData);
        }

        public void AddWorld(World world)
        {
            if (world != null)
            {
                var userDataWorld = _userData.Worlds.FirstOrDefault(x => x.Url.ToLower() == world.ContentPath.ToLower());
                if (userDataWorld == null)
                {
                    userDataWorld = new UserDataWorld();
                    _userData.Worlds.Insert(0, userDataWorld);
                }

                userDataWorld.SetFromWorld(world);
                Save();
            }
        }

        public void DeleteWorld(UserDataWorld userDataWorld)
        {
			if (userDataWorld != null)
			{
				var existingWorld = this.UserData.Worlds.FirstOrDefault(x => x.Url.ToLower() == userDataWorld.Url.ToLower());
				if (existingWorld != null)
				{
					this.UserData.Worlds.Remove(existingWorld);
					Save();
				}
			}
        }

		public void MoveWorld(int fromPos, int toPos)
		{
			var world = this.UserData.Worlds[fromPos];
			// are we inserting 
			if (toPos < fromPos) 
			{
				// add one to where we delete, because we're increasing the index by inserting
				fromPos += 1;
			} 
			else 
			{
				// add one to where we insert, because we haven't deleted the original yet
				toPos += 1;
			}
			this.UserData.Worlds.Insert (toPos, world);
			this.UserData.Worlds.RemoveAt (fromPos);
		}

		public bool IsFavouriteWorld(UserDataWorld world)
		{
			return this.UserData.FavouriteWorlds.Any(x => x.Url.ToLower() == world.Url.ToLower());
		}

		public bool CanAddWorldToFavourites(UserDataWorld world)
		{
			return (world != null && !this.UserData.FavouriteWorlds.Any(x => x.Url.ToLower() == world.Url.ToLower()) && this.UserData.FavouriteWorlds.Count < 2);
		}

		public bool AddWorldToFavourites(UserDataWorld world)
		{
			if (CanAddWorldToFavourites(world))
			{
				this.UserData.FavouriteWorlds.Add(world);
				return true;
			}

			return false;
		}

		public bool RemoveWorldFromFavourites(UserDataWorld world)
		{
			var existingWorld = this.UserData.FavouriteWorlds.FirstOrDefault(x => x.Url.ToLower() == world.Url.ToLower());
			if (existingWorld != null)
			{
				this.UserData.FavouriteWorlds.Remove(existingWorld);
				return true;
			}

			return false;
		}

		private void TEMP_AddTestData()
		{
			//TODO: Adding dummy data for testing purposes. Remove later
			if (!_userData.Worlds.Any(x => x.Title == "The Frozen Edge"))
				_userData.Worlds.Add(new UserDataWorld 
					{ 
						Title = "The Frozen Edge", 
						Subtitle = "A world on the edge of midnight where a brave few hold back the tide of darkness", 
						ImageUrl = "/media/1007/frozen-thumb.jpg",
						Url = "http://www.blackdragongames.org/worlds/frozen",
						LastAccessedStatusCode = StatusCode.Ok
					});

			if (!_userData.Worlds.Any(x => x.Title == "The Dungeons of Numptor"))
				_userData.Worlds.Add(new UserDataWorld 
					{ 
						Title = "The Dungeons of Numptor", 
						Subtitle = "Who says wizards are smart? When a good one has a bad idea he can't let go, adventurers pay the price!", 
						ImageUrl = "/media/1008/numptor-thumb.jpg",
						Url = "http://www.blackdragongames.org/worlds/numptor",
						LastAccessedStatusCode = StatusCode.Ok
					});
		}
    }
}
