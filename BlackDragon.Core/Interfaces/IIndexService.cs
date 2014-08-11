using System;
using BlackDragon.Core.Entities;

namespace BlackDragon.Core
{
    public interface IIndexService
    {
        void Clear();
        void ClearForBaseUrl(string baseUrl);
        void Load(World world);
        void Load(Adventure adventure);
        T Get<T>(string contentPath) where T : IdentifiableItem;
    }
}

