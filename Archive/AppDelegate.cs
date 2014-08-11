using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlackDragon.Core.IoC;
using BlackDragon.Core;
using BlackDragon.Fx;
using BlackDragon.Fx.MapGL;
using BlackDragon.Core.DeepZoom;

namespace BlackDragon.Archive
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        AppController appController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            RegisterDependencies();
			//MapTileHelper.InitializeOpenGL();

			var fileService = DC.Get<IFileAccessService>();
			fileService.Initialize(true);
			fileService.CreateCacheFolder(SeadragonTileSource.LowResFolder);

			Settings.IsRetina = (UIScreen.MainScreen.Scale > 1.0);
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            appController = new AppController();
            appController.PushViewController(DC.Get<MenuViewController>(), false);

            window.RootViewController = appController;

            window.MakeKeyAndVisible();


            return true;
        }

        private void RegisterDependencies()
        {
            DC.RegisterCoreDependencies();

            var inst = TinyIoCContainer.Current;
            
			//inst.Register<IImageSource<UIImage>, BDGImageSource>();
            inst.Register<WorldViewController>();
            inst.Register<AdventureViewController>();
            inst.Register<MenuViewController>();
			inst.Register<WorldManagementTableViewSource>();
        }
    }
}

