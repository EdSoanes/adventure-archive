using BlackDragon.Core.Entities;
using BlackDragon.Umbraco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using umbraco;
using umbraco.NodeFactory;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace BlackDragon.CMS.Controllers
{
    public class WorldsController : ApiController
    {
        //
        // GET: /Worlds/[WorldName]
        public World Get(string id)
        {
            var worldNodes = uQuery.GetNodesByType("World");
            var worldNode = worldNodes.FirstOrDefault(x => x.Name.ToLower() == id.ToLower());
            if (worldNode != null)
            {
                var generator = new JsonGenerator();
                var world = generator.GetWorld(worldNode);

                return world;
            }

            return null;
        }

        //
        // GET: /Worlds/
        public IEnumerable<World> GetAll()
        {
            var worlds = new List<World>();

            var worldNodes = uQuery.GetNodesByType("World");
            var generator = new JsonGenerator();
            foreach (var worldNode in worldNodes)
            {
                var world = generator.GetWorld(worldNode);
                if (world != null)
                    worlds.Add(world);
            }
            return worlds;
        }
    }
}
