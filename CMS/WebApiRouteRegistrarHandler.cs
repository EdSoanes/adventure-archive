using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace BlackDragon.CMS
{
    public class WebApiRouteRegistrarHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        private void Document_BeforePublish(Document sender, PublishEventArgs e)
        {
            //Do what you need to do. In this case logging to the Umbraco log
            LogHelper.Debug(this.GetType(), "the document " + sender.Text + " is about to be published");

            //cancel the publishing if you want.
            e.Cancel = true;
        }
    }
}