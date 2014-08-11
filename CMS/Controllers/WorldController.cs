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
            var rootNode = new Node(-1);
            var worldNodes = rootNode.PublishedChildren().Where(x => x.NodeTypeAlias == "World");
            var worldNode = worldNodes.FirstOrDefault(x => x.Name.ToLower() == id.ToLower());
            if (worldNode != null)
            {
                var generator = new JsonGenerator();
                var world = generator.GetWorld(worldNode);

                return world;
            }

            return null;
        }

        public static IPublishedContent GetNodeByAlias(string alias)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var contentNode = umbracoHelper.TypedContentSingleAtXPath(String.Format("//{0}", alias));

            return contentNode;
        }
    }
}
