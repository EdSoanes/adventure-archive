using System;
using System.Linq;
using System.Collections.Generic;
using BlackDragon.Core.Entities;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BlackDragon.Core
{
    public class WorldService : IWorldService
    {
		private readonly IFileAccessService _fileService;
        private readonly IUserDataService _userDataService;

        public World SelectedWorld
        {
            get;
            set;
        }

		public WorldService(IFileAccessService fileSource, IUserDataService userDataService)
        {
			_fileService = fileSource;
            _userDataService = userDataService;
        }

		public Action LoadAction(Action<StatusCode> completionAction = null)
        {
			return () =>
			{
				StatusCode res = StatusCode.Ok;

				try
				{
					var userDataWorld = _userDataService.UserData.SelectedWorld;
					if (userDataWorld != null)
					{
						var url = userDataWorld.AbsoluteContentPath;
						Uri uri = new Uri(url.WithHttpProtocol());
						if (uri.Segments.Last().ToLower() != "json")
							uri = new Uri(uri, "json");


						var tcs = new TaskCompletionSource<FileCacheEntry>();
						_fileService.Request(uri.OriginalString, (fileCacheEntry) =>
							{
								tcs.SetResult(fileCacheEntry);
							});

						Task.WaitAll(new Task[] { tcs.Task });
						var json = tcs.Task.Result.GetData<string>();

						//var json = _fileService.GetTextFile(uri);
						var world = JsonConvert.DeserializeObject<World>(json);
                        PrepareWorld(url, world);
						if (world != null)
							SelectedWorld = world;
						else
							res = StatusCode.JsonDeserializeFailed;
					}
					else
						res = StatusCode.UserDataWorldNotSelected;
				}
				catch (Exception ex)
				{
					res = ExceptionHandler.HttpDownload(ex);
				}
				finally
				{
					if (completionAction != null)
						completionAction.Invoke(res);
				}
			};
        }

        private void PrepareWorld(string baseUrl, World world)
        {
            world.Pages.ForEach(x =>
            {
                Uri uri = new Uri(baseUrl.WithHttpProtocol());
                uri = new Uri(uri, x.ContentPath);
                x.AbsoluteContentPath = uri.AbsoluteUri;
            });
        }
    }
}

