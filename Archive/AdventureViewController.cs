using System;
using System.Drawing;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BlackDragon.Fx;

namespace BlackDragon.Archive
{
    [Register("AdventureViewController")]
    public class AdventureViewController : BDGViewController
    {
        public AdventureViewController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = new AdventureView();

            var backbtn = new UIButton();
            backbtn.Frame = new RectangleF(100, 200, 200, 50);
            backbtn.SetTitle("Back", UIControlState.Normal);
            backbtn.BackgroundColor = UIColor.White;
            backbtn.SetTitleColor(UIColor.Black, UIControlState.Normal);

            backbtn.TouchUpInside += (s1, e1) =>
            {
                this.NavigationController.PopViewControllerAnimated(true);
            };

            View.AddSubview(backbtn);

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }
    }
}