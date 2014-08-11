using BlackDragon.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Core
{
    public interface IUserDataContentService
    {
        List<UserDataWorld> WorldData { get; }

        void LoadWorlds();
        void SaveWorlds();
        void AddWorld(World world);
        void DeleteWorld(UserDataWorld userDataWorld);

        void LoadWorldUserData(string worldBasePath);
        void SaveWorldUserData();
        UserDataContent GetWorldUserDataItem(string contentPath);
        void SetWorldUserDataItem(UserDataContent userDataContent);
    }
}
