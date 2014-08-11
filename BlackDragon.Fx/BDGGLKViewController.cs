using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;

using BlackDragon.Core.TPL;
using MonoTouch.GLKit;

namespace BlackDragon.Fx
{
    public class BDGGLKViewController : GLKViewController
    {
        TaskFactory _factory;

        protected TaskFactory TaskFactory
        {
            get
            {
                if (_factory == null)
                    _factory = new TaskFactory(new OrderedTaskScheduler());

                return _factory;
            }
        }

        public BDGGLKViewController()
            : base()
        {
        }
    }
}

