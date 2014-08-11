using BlackDragon.Core.Entities;
using BlackDragon.Umbraco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using umbraco.NodeFactory;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace BlackDragon.CMS.Controllers
{
    public class WorldController : UmbracoApiController
    {
        //
        // GET: /World/
        [HttpGet]
        public World GetWorldData(string id)
        {
            var worldNode = GetNodeByAlias(id);
            var generator = new JsonGenerator();
            var world = generator.GetWorld(new Node(worldNode.Id));

            return world;
        }

        public static IPublishedContent GetNodeByAlias(string alias)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var contentNode = umbracoHelper.TypedContentSingleAtXPath(String.Format("//{0}", alias));

            return contentNode;
        }
    }
}
