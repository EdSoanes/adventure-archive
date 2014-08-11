using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using MonoTouch.CoreAnimation;

namespace BlackDragon.Fx.Gestures
{
    public class ShouldDragEventArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }

        public PointF Location
        {
            get;
            private set;
        }

        public ShouldDragEventArgs(bool cancel, PointF location) : base()
        {
            Cancel = cancel;
            Location = location;
        }
    }

    public class HorizontalViewDragEventArgs : EventArgs
    {
        public float CurrentX
        {
            get;
            private set;
        }

        public float LeftViewX
        {
            get;
            private set;
        }

        public float RightViewX
        {
            get;
            private set;
        }

        public float Span
        {
            get { return RightViewX - LeftViewX; }
        }

		public bool Expanding
		{
			get;
			private set;
		}
            
		public HorizontalViewDragEventArgs(float currentX, float leftViewX, float rightViewX, bool expanding)
        {
            CurrentX = currentX;
            LeftViewX = leftViewX;
            RightViewX = rightViewX;
			Expanding = expanding;
        }
    }

    /// <summary>
    /// SHow navigation menu drag gesture recognizer.
    /// </summary>
    public class HorizontalViewDragGestureRecognizer : UIPanGestureRecognizer
    {
        public EventHandler<ShouldDragEventArgs> ShouldDrag;
        public EventHandler<HorizontalViewDragEventArgs> DragBegin;
        public EventHandler<HorizontalViewDragEventArgs> Dragging;
        public EventHandler<HorizontalViewDragEventArgs> DragEnd;

        public HorizontalViewDragGestureRecognizer(UIView mainView, UIView view, RectangleF dragHotspot, float leftViewX, float rightViewX)
            : base()
        {
            this.MinimumNumberOfTouches = 1;
            this.MaximumNumberOfTouches = 1;

            var d = new HorizontalViewDragGestureDelegate(mainView, view, dragHotspot, leftViewX, rightViewX);
            d.DragBegin += (s1, e1) =>
            {
                if (DragBegin != null)
                    DragBegin.Invoke(this, e1);
            };

            d.Dragging += (s2, e2) =>
            {
                if (Dragging != null)
                    Dragging.Invoke(this, e2);
            };

            d.DragEnd += (s3, e3) =>
            {
                if (DragEnd != null)
                    DragEnd.Invoke(this, e3);
            };
            d.ShouldDrag += (s4, e4) =>
            {
                if (ShouldDrag != null)
                    ShouldDrag.Invoke(this, e4);
            };

            Delegate = d;
            AddTarget(d, new Selector("HandleHorizontalDrag"));
        }

		public void SetBoundaries(float leftViewX, float rightViewX, RectangleF? dragHotspot = null)
        {
            var d = Delegate as HorizontalViewDragGestureDelegate;
            if (d != null)
				d.SetBoundaries(leftViewX, rightViewX, dragHotspot);
        }
    }

    /// <summary>
    /// Rpg bookmark tap gesture delegate.
    /// </summary>
    public class HorizontalViewDragGestureDelegate : UIGestureRecognizerDelegate
    {
        public EventHandler<ShouldDragEventArgs> ShouldDrag;
        public EventHandler<HorizontalViewDragEventArgs> DragBegin;
        public EventHandler<HorizontalViewDragEventArgs> Dragging;
        public EventHandler<HorizontalViewDragEventArgs> DragEnd;

        private float _currentX = 0f;
        private DateTime _prevElapsedTime;
        private TimeSpan _currentTimeSpan;
        private float _currentPixelSpan;

        private UIView _mainView;
        private UIView _view;
        private RectangleF _dragHotspot;
        private float _leftViewX;
        private float _rightViewX;

        public HorizontalViewDragGestureDelegate(UIView mainView, UIView view, RectangleF dragHotspot, float leftViewX, float rightViewX)
        {
            _mainView = mainView;
            _view = view;
            _dragHotspot = dragHotspot;
			SetBoundaries(leftViewX, rightViewX);
        }

		public void SetBoundaries(float leftViewX, float rightViewX, RectangleF? dragHotspot = null)
        {
            _leftViewX = leftViewX;
            _rightViewX = rightViewX;
			if (dragHotspot.HasValue)
				_dragHotspot = dragHotspot.Value;
        }

        private void OnDragBegin()
        {
            if (DragBegin != null)
            {
				var e = new HorizontalViewDragEventArgs(_view.Frame.X, _leftViewX, _rightViewX, _currentPixelSpan <= 0);
                DragBegin.Invoke(this, e);
            }
        }

        private void OnDragging()
        {
            if (Dragging != null)
            {
				var e = new HorizontalViewDragEventArgs(_view.Frame.X, _leftViewX, _rightViewX, _currentPixelSpan <= 0);
                Dragging.Invoke(this, e);
            }
        }

        private void OnDragEnd()
        {
            if (DragEnd != null)
            {
				var e = new HorizontalViewDragEventArgs(_view.Frame.X, _leftViewX, _rightViewX, _currentPixelSpan <= 0);
                DragEnd.Invoke(this, e);
            }
        }

        public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            return true;
        }

        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            return true;
        }

        public override bool ShouldBegin(UIGestureRecognizer recognizer)
        {
            var location = recognizer.LocationInView(_view);
            var cancel = !_dragHotspot.Contains(location);

            var e = new ShouldDragEventArgs(cancel, location);
            if (ShouldDrag != null)
                ShouldDrag.Invoke(this, e);

            return !e.Cancel;
        }

        [Export("HandleHorizontalDrag")]
        public void HandleHorizontalDrag(UIPanGestureRecognizer recognizer)
        {
            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    HandleBeginState(recognizer);
                    break;

                case UIGestureRecognizerState.Changed:
                    HandleChangedState(recognizer);
                    break;

                case UIGestureRecognizerState.Ended:
                    HandleEndState(recognizer);
                    break;

                case UIGestureRecognizerState.Cancelled:
                    HandleCancelledState(recognizer);
                    break;
            }
        }

        private void HandleBeginState(UIPanGestureRecognizer recognizer)
        {
            _currentX = GetLocationXInMainView(recognizer);
            _prevElapsedTime = DateTime.Now;

            OnDragBegin();
        }

        private void HandleChangedState(UIPanGestureRecognizer recognizer)
        {
            var x = GetLocationXInMainView(recognizer);
            var currElapsedTime = DateTime.Now;

            _currentTimeSpan = currElapsedTime - _prevElapsedTime;
			_currentPixelSpan = (x - _currentX);// * (_view.IsUpsideDown() ? -1 : 1);
            _currentX = x;

            _prevElapsedTime = currElapsedTime;

            var minX = Math.Min(_leftViewX, _rightViewX);
            var maxX = Math.Max(_leftViewX, _rightViewX);
            _view.Frame = _view.Frame.MoveX(_currentPixelSpan, minX, maxX);

            OnDragging();
        }

        private void HandleEndState(UIPanGestureRecognizer recognizer)
        {
			OnDragEnd();
//            if (_currentPixelSpan > 0)
//                MoveToEndStateRight();
//            else
//                MoveToEndStateLeft();
        }

//        public void MoveToEndStateLeft()
//        {
//			AnimateViewTo(_view.Frame.MoveToX(_leftViewX));
//			//_view.Frame = _view.Frame.MoveToX(_leftViewX);
//			//OnDragEnd();
//        }
//
//        public void MoveToEndStateRight()
//        {
//			AnimateViewTo(_view.Frame.MoveToX(_rightViewX));
//			//_view.Frame = _view.Frame.MoveToX(_rightViewX);
//			//OnDragEnd();
//        }
//
        private void HandleCancelledState(UIPanGestureRecognizer recognizer)
        {
            HandleEndState(recognizer);
        }

        private float GetLocationXInMainView(UIPanGestureRecognizer recognizer)
        {
            var location = recognizer.LocationInView(_mainView);
			var x = location.X; //_mainView.IsPortrait() ? location.X : location.Y;
            return x;
        }

//		private void AnimateViewTo(RectangleF newFrame)
//		{
//			UIView.Animate (0.15f, 0, UIViewAnimationOptions.CurveEaseOut ,
//				() => { _view.Frame = newFrame; },
//				() => 
//				{ 
//					_view.Frame = newFrame; 
//					OnDragEnd();
//				}
//			);
//		}
    }
}