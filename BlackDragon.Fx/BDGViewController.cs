using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;

using BlackDragon.Core.TPL;

namespace BlackDragon.Fx
{
    public class BDGViewController : UIViewController
    {
        TaskFactory _factory;

        protected TaskFactory TaskFactory
        {
            get
            {
                if (_factory == null)
                    _factory = new System.Threading.Tasks.TaskFactory(new OrderedTaskScheduler());

                return _factory;
            }
        }

        public BDGViewController()
            : base()
        {
        }

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}
    }
}

