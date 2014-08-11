using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlackDragon.Fx
{
    public class TabPopoutActionEventArgs : EventArgs
    {
        public object Action
        {
            get;
            private set;
        }

        public TabPopoutActionEventArgs(object action)
        {
            Action = action;
        }
    }

    public interface ITabPopoutContentView
    {
        event EventHandler ContentLoaded;
        event EventHandler<TabPopoutActionEventArgs> ActionRaised;
        void LoadContent(object content);
        bool ShouldLoadContent(object content);
    }
}