using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Core.IoC
{
    public class DC
    {
        public static T Get<T>() where T : class
        {
            return TinyIoCContainer.Current.Resolve<T>();
        }

        public static void RegisterCoreDependencies()
        {
            var inst = TinyIoCContainer.Current;

			inst.Register<ILogService, LogService>();
			inst.Register<IFileAccessService, FileAccessService>();
            inst.Register<IUserDataService, UserDataService>();
            inst.Register<IWorldService, WorldService>();
            inst.Register<IAdventureService, AdventureService>();
            inst.Register<IIndexService, IndexService>();
            inst.Register<INavigationService, NavigationService>();
        }
    }
}
