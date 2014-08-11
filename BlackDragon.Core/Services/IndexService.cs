using System;
using System.Linq;
using System.Collections.Generic;
using BlackDragon.Core.Entities;

namespace BlackDragon.Core
{
    public class IndexService : IIndexService
    {
        private readonly IWorldService _worldService;

        private static Dictionary<string, IdentifiableItem> Index
        {
            get;
            set;
        }

        public IndexService(IWorldService worldService)
        {
            _worldService = worldService;
            Index = new Dictionary<string, IdentifiableItem>();
        }

        public void Clear()
        {
            Index.Clear();
        }

        public void ClearForBaseUrl(string baseUrl)
        {
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                var keysToRemove = Index.Keys.Where(x => x.StartsWith(baseUrl)).ToList();
                foreach (var key in keysToRemove)
                    Index.Remove(key);
            }
        }

        public void Load(World world)
        {
            if (world != null) 
            {
                Add(world.Map);

                foreach (var adventure in world.Adventures)
                    Add(adventure);

                foreach (var page in world.Pages)
                    Add(page);

                foreach (var reference in world.References)
                    Add(reference);
            }
        }

        public void Load(Adventure adventure)
        {
            if (adventure != null)
            {
                foreach (var reference in adventure.References)
                    Add(reference);

                foreach (var map in adventure.Maps)
                    Add(map);

                foreach (var chapter in adventure.Chapters)
                {
                    Add(chapter);

                    foreach (var page in chapter.Pages)
                    {
                        Add(page);
                        if (!string.IsNullOrWhiteSpace(page.MapContentPath))
                            page.Map = Get<Map>(page.MapContentPath);
                    }
                }
            }
        }

        private void Add(IdentifiableItem item)
        {
            if (item != null)
            {
                //item.SetRootPath(_worldService.Domain);

                Console.WriteLine("Indexing: " + item.GetType().ToString() + item.ContentPath);

                if (!Index.ContainsKey(item.ContentPath))
                    Index.Add(item.ContentPath, item);
                else
                    Index[item.ContentPath] = item;
            }
        }

        public T Get<T>(string contentPath) where T : IdentifiableItem
        {
            var key = contentPath;
            if (Index.ContainsKey(key))
                return Index[key] as T;
            else
                return null;
        }
    }
}

