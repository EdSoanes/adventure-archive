using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Fx
{
    public class BDGDisplayLink
    {
        private DateTime _lastStartTime = DateTime.Now;

        private object _requestedStopLock = new object();
        private bool _requestedStop;

        private object _lockObject = new object();
        private CADisplayLink _displayLink;

        private BDGDisplayLink()
        {
            _displayLink = CADisplayLink.Create(() =>
            {
                lock (_lockObject)
                {
                    if (Run != null)
                        Run.Invoke(this, new EventArgs());

//                    if (DateTime.Now > _lastStartTime.AddMilliseconds(500))
//                    {
//                        _displayLink.Paused = true;
//
//                        if (Stopped != null)
//                            Stopped.Invoke(this, new EventArgs());
//
//                        Console.WriteLine("Stopped map display link");
//                    }
                }
            });

            _displayLink.Paused = true;
            _displayLink.FrameInterval = 1;
            _displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoop.NSRunLoopCommonModes);
        }

        private static BDGDisplayLink _link = null;
        public static BDGDisplayLink Link
        {
            get 
            {
                if (_link == null)
                    _link = new BDGDisplayLink();

                return _link; 
            }
        }

        private EventHandler Run;
        private EventHandler Stopped;

        public void RegisterRunHandler(EventHandler handler)
        {
            lock (_lockObject)
                Run += handler;
        }

        public void UnregisterRunHandler(EventHandler handler)
        {
			lock (_lockObject)
			{
				Run -= handler;
				if (Run == null)
					Stop();
			}
        }

        public void RegisterStoppedHandler(EventHandler handler)
        {
            lock (_lockObject)
                Stopped += handler;
        }

        public void UnregisterStoppedHandler(EventHandler handler)
        {
            lock (_lockObject)
                Stopped -= handler;
        }

        public void RequestStart()
        {
            lock (_lockObject)
            {
                _lastStartTime = DateTime.Now;
                if (_displayLink.Paused)
                {
                    _displayLink.Paused = false;

                    Console.WriteLine("Starting map display link");
                }
            }
        }

        public void Stop()
        {
            lock (_lockObject)
                _displayLink.Paused = true;
        }
    }
}
