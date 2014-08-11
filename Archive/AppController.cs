using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlackDragon.Archive
{
    [Register("AppController")]
    public class AppController : UINavigationController
    {
        public AppController()
        {
            UIApplication.SharedApplication.SetStatusBarHidden(true, false);
            SetNavigationBarHidden(true, false);
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}