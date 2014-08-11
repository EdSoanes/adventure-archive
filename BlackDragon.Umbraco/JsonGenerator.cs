using BlackDragon.Core;
using BlackDragon.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using umbraco;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace BlackDragon.Umbraco
{
    public class JsonGenerator
    {
        public string GetWorldJson()
        {
            var node = Node.GetCurrent();

            var world = GetWorld(node);

            var json = JsonConvert.SerializeObject(world);
            return json;
        }

        public World GetWorld(INode node)
        {
            var world = new World();

            SetIdentifiableItemFields(node, world);
            world.Copyright = node.Get<string>("copyright");

            foreach (var n in node.PublishedChildren())
            {
                if (n.NodeTypeAlias == "Map")
                {
                    var map = GetMap(n);
                    world.Map = map;
                }
                else if (n.NodeTypeAlias == "Page")
                {
                    var page = GetPage(n);
                    world.Pages.Add(page);
                }
                else if (n.NodeTypeAlias == "Reference")
                {
                    var reference = GetReference(n);
                    world.References.Add(reference);
                }
                else if (n.NodeTypeAlias == "Adventure")
                {
                    var adventure = GetAdventure(n);
                    world.Adventures.Add(adventure);
                }
            }

            return world;
        }

        public string GetAdventureJson()
        {
            var node = Node.GetCurrent();
            var adventure = GetAdventure(node);

            var json = JsonConvert.SerializeObject(adventure);
            return json;
        }

        private Adventure GetAdventure(INode node)
        {
            var adventure = new Adventure();

            SetIdentifiableItemFields(node, adventure);
            adventure.Copyright = node.Get<string>("copyright");
            adventure.RpgSystem = node.Get<string>("rpgSystem");

            foreach (var n in node.PublishedChildren())
            {
                if (n.NodeTypeAlias == "Map")
                {
                    var map = GetMap(n);
                    adventure.Maps.Add(map);
                }
                else if (n.NodeTypeAlias == "Chapter")
                {
                    var chapter = GetChapter(n);
                    adventure.Chapters.Add(chapter);
                }
                else if (n.NodeTypeAlias == "Reference")
                {
                    var reference = GetReference(n);
                    adventure.References.Add(reference);
                }
            }

            return adventure;
        }

        private Page GetPage(INode node)
        {
            var page = new Page();

            SetIdentifiableItemFields(node, page);
            var linkNode = node.GetLink("mapLink");
            if (linkNode != null)
                page.MapContentPath = linkNode.NiceUrl;

            return page;
        }

        private Reference GetReference(INode node)
        {
            var reference = new Reference();

            SetIdentifiableItemFields(node, reference);

            return reference;
        }

        private Map GetMap(INode node)
        {
            var map = new Map();

            SetIdentifiableItemFields(node, map);
            map.FileExtension = node.Get<string>("fileExtension");
            map.GeneratorApp = node.Get<string>("generatorApp");
            map.Height = node.Get<int>("height");
            map.Width = node.Get<int>("width");
            map.MapFilesPath = GetWebSiteDomainName().CombineUrl(node.Get<string>("mapFilesPath"));

            var mapMarkerNodes = node.PublishedChildren().Where(x => x.NodeTypeAlias == "MapMarker").ToArray(); 
            for (int i = 0; i < mapMarkerNodes.Count(); i++)
            {
                var mapMarkerNode = mapMarkerNodes[i];
                var mapMarker = GetMapMarker(mapMarkerNode, i + 1);
                if (mapMarker != null)
                    map.Markers.Add(mapMarker);
            }

            return map;
        }

        private MapMarker GetMapMarker(INode node, int nodeNumber)
        {
            var linkNode = node.GetLink("link");
            if (linkNode != null)
            {
                var mapMarker = new MapMarker();

                mapMarker.Identifier = node.Id.ToString();
                mapMarker.Number = nodeNumber;
                mapMarker.Type = node.Get<string>("markerType");
                mapMarker.X = node.Get<float>("x");
                mapMarker.Y = node.Get<float>("y");

                mapMarker.Title = linkNode.Get<string>("title");
                mapMarker.Subtitle = linkNode.Get<string>("subtitle");
                mapMarker.ContentPath = linkNode.NiceUrl;

                return mapMarker;
            }

            return null;
        }

        private Chapter GetChapter(INode node)
        {
            var chapter = new Chapter();

            SetIdentifiableItemFields(node, chapter);

            foreach (var n in node.PublishedChildren())
            {
                if (n.NodeTypeAlias == "Page")
                {
                    var page = GetPage(n);
                    chapter.Pages.Add(page);
                }
            }

            return chapter;
        }

        private void SetIdentifiableItemFields(INode node, IdentifiableItem item)
        {
            item.Identifier = node.Id.ToString();
            item.ContentPath = node.NiceUrl;
            item.Title = node.Get<string>("title");
            item.Subtitle = node.Get<string>("subtitle");
            item.ImageUrl = node.GetMediaUrl("image");            
        }

        private void SetLocatableItemFields(INode node, LocatableItem item)
        {
            SetIdentifiableItemFields(node, item);

            item.X = node.Get<float>("x");
            item.Y = node.Get<float>("y");
            item.Locked = node.Get<bool>("locked");

            var linkNode = node.GetLink("sectionLink");
            if (linkNode != null)
                item.ContentPath = linkNode.NiceUrl;
        }

        /// <summary>
        /// Get the current domain name of the website.
        /// </summary>
        /// <remarks>
        /// No use inside an iOS app! Only useful in an asp.net website context
        /// </remarks>
        /// <returns></returns>
        private string GetWebSiteDomainName()
        {
            var sPath = HttpContext.Current.Request.Url.ToString().Replace("http://", "");
            string url;
            if (sPath.Contains("/"))
            {
                var strarry = sPath.Split('/');
                url = strarry[0];
            }
            else
            {
                url = sPath;
            }
            return string.Concat("http://", url);
        }
    }
}
