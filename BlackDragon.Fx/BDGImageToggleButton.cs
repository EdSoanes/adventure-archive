using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlackDragon.Fx
{
	public class BDGImageToggleButton : UIButton
	{
		public event EventHandler<ToggleEventArgs> Toggling;
		public event EventHandler<ToggleEventArgs> Toggled;

		public bool IsOn
		{
			get;
			private set;
		}

		public object Data
		{
			get;
			private set;
		}

		UIImage _imageOn = null;
		UIImage _imageOff = null;

		public BDGImageToggleButton(object data = null)
			: base()
		{
			Data = data;
			InitializeView();
		}

		public BDGImageToggleButton(UIImage imageOff, UIImage imageOn, object data = null)
			: base()
		{
			_imageOn = imageOn;
			_imageOff = imageOff;
			Data = data;
			InitializeView();
		}

		public void InitializeView()
		{
			this.BackgroundColor = UIColor.Clear;
			this.TintColor = UIColor.Clear;

			this.TouchUpInside += (s1, e1) => Toggle();
			SetBackground();
		}

		public void SetButtonData(bool toggleOn, object data, UIImage imageOn = null, UIImage imageOff = null)
		{
			IsOn = toggleOn;
			Data = data;

			if (imageOn != null)
				_imageOn = imageOn;

			if (imageOff != null)
				_imageOff = imageOff; 

			SetBackground();
		}

		public void SetToggle(bool on)
		{
			IsOn = on;
			SetBackground();
			if (Toggled != null)
				Toggled.Invoke(this, new ToggleEventArgs(IsOn, Data));
		}

		private void Toggle()
		{
			var togglingArgs = new ToggleEventArgs(IsOn, Data);
			if (Toggling != null)
				Toggling.Invoke(this, togglingArgs);

			if (!togglingArgs.Cancel)
			{
				IsOn = !IsOn;
				SetBackground();
				if (Toggled != null)
					Toggled.Invoke(this, new ToggleEventArgs(IsOn, Data));
			}
		}

		private void SetBackground()
		{
			if (_imageOn != null && _imageOff != null)
			{
				if (IsOn)
					this.SetBackgroundImage(_imageOn, UIControlState.Normal);
				else
					this.SetBackgroundImage(_imageOff, UIControlState.Normal);
			}
		}
	}

	public class ToggleEventArgs : EventArgs
	{
		public bool Cancel
		{
			get;
			set;
		}

		public bool IsOn
		{
			get;
			private set;
		}

		public object Data
		{
			get;
			private set;
		}

		public ToggleEventArgs(bool isOn, object data)
			: base()
		{
			IsOn = isOn;
			Data = data;
		}
	}
}

