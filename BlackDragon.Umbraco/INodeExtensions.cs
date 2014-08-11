using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.interfaces;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.media;

namespace BlackDragon.Umbraco
{
    public static class INodeExtensions
    {
        public static IEnumerable<INode> PublishedChildren(this INode node)
        {
            if (node == null)
                return Enumerable.Empty<INode>();
            else
                return node.ChildrenAsList.Select(x => x.PublishedNode()).Where(x => x != null).OrderBy(x => x.SortOrder);
        }

        public static Node PublishedNode(this INode node)
        {
            if (node != null)
                return new Node(node.Id);
            else
                return null;
        }

        public static T Get<T>(this INode node, string propertyName)
        {
            var prop = node.GetProperty(propertyName);
            if (prop != null)
            {
                if (string.IsNullOrEmpty(prop.Value))
                    return default(T);

                if (typeof(T) == typeof(bool))
                {
                    if (prop.Value == "1")
                        return (T)Convert.ChangeType(true, typeof(T));
                    else
                        return (T)Convert.ChangeType(false, typeof(T));
                }

                return (T)Convert.ChangeType(prop.Value, typeof(T));
            }
            return default(T);
        }

        public static INode GetLink(this INode node, string propertyName)
        {
            var link = node.Get<int>(propertyName);
            if (link != 0)
            {
                var linkNode = new Node(link);
                return linkNode;
            }

            return null;
        }

        public static string GetMediaUrl(this INode node, string propertyName)
        {
            var mediaId = node.Get<int>(propertyName);
            if (mediaId != 0)
            {
                Media mymedia = new Media(mediaId);
                return mymedia.getProperty("umbracoFile").Value.ToString();
            }

            return string.Empty;
        }
    }
}
