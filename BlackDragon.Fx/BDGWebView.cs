using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlackDragon.Core.IoC;
using System.Drawing;

namespace BlackDragon.Fx
{
    public class BDGWebView : UIWebView, ITabPopoutContentView
    {
        private BDGWebViewDelegate _delegate = new BDGWebViewDelegate();

        private string _currentUrl;

        public BDGWebView ()
		{
			Initialize();
		}

        public BDGWebView(RectangleF frame)
            : base(frame)
        {
            Initialize();
        }

		private void Initialize()
		{
			this.Delegate = _delegate;
            this.ScalesPageToFit = true;

			//Hides the default shadows around the displayed web page
			foreach (var iv in this.Subviews[0].Subviews.Where(x => x is UIImageView))
				iv.Hidden = true;

			//Prevents the default bounce when scrolling a web page
			var subView = this.Subviews.Where(x => x is UIScrollView).FirstOrDefault() as UIScrollView;
			if (subView != null)
				subView.Bounces = false;
		}

        #region ITabPopoutContentView implementation

        public event EventHandler ContentLoaded;
        public event EventHandler<TabPopoutActionEventArgs> ActionRaised;

        public void LoadContent(object content)
        {
            string contentUrl = content as string;
            if (contentUrl == null)
                throw new ArgumentException("content is null or not a string", "content");

            if (string.IsNullOrEmpty(_currentUrl) || _currentUrl.ToLower() != contentUrl.ToLower())
            {
                _currentUrl = contentUrl.ToLower();
                if (_currentUrl.EndsWith("/"))
                    _currentUrl = _currentUrl.TrimEnd(new char[] { '/' });

                if (!string.IsNullOrWhiteSpace(contentUrl))
                {
                    // create url and request
                    NSUrl url = new NSUrl(contentUrl);
                    NSUrlRequest request = new NSUrlRequest(url, NSUrlRequestCachePolicy.ReloadIgnoringCacheData, 10);
                    // load request and add to main ?view
                    this.LoadRequest(request);
                }
            }
        }

        public bool ShouldLoadContent(object content)
        {
            var contentUrl = content as string;
            if (contentUrl == null)
                return false;

            if (contentUrl.EndsWith("/"))
                contentUrl = contentUrl.TrimEnd(new char[] { '/' });

            var shouldNavigate = (!string.IsNullOrEmpty(_currentUrl) && contentUrl.ToLower() == _currentUrl.ToLower());

            //If not then raise an action with the url to be handled
            if (!shouldNavigate)
                RaiseActionRaised(contentUrl);

            return shouldNavigate;
        }

        protected void RaiseActionRaised(string url)
        {
            if (ActionRaised != null)
            {
                var e = new TabPopoutActionEventArgs(url);
                ActionRaised.Invoke(this, e);
            }
        }

        public void RaiseContentLoaded()
        {
            if (ContentLoaded != null)
                ContentLoaded.Invoke(this, new EventArgs());
        }
        
        #endregion ITabPopoutContentView implementation
    }

	public class BDGWebViewDelegate : UIWebViewDelegate
	{
		public BDGWebViewDelegate ()
		{
		}

		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			var baseWebView = webView as BDGWebView;
			if (baseWebView != null)
				return baseWebView.ShouldLoadContent(request.Url.AbsoluteString);

			return false;
		}
		
		public override void LoadFailed(UIWebView webView, NSError error)
		{
			//var err = error;
			
			// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
		}

		public override void LoadingFinished(UIWebView webView)
		{
			var baseWebView = webView as BDGWebView;
			
			if (baseWebView != null)
				baseWebView.RaiseContentLoaded();
		}
	}
}
