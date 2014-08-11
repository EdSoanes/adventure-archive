using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BlackDragon.Core.Entities;

namespace BlackDragon.Core
{
    public interface IUserDataService
    {
        UserData UserData { get; }

		Action LoadAction(Action<StatusCode> completeAction = null);
		Action LoadNewWorldAction(string url, Action completeAction = null);
        void Load();
        void Save();

        void AddWorld(World world);
        void DeleteWorld(UserDataWorld userDataWorld);
		void MoveWorld(int fromPos, int toPos);

		bool CanAddWorldToFavourites(UserDataWorld world);
		bool IsFavouriteWorld(UserDataWorld world);
		bool AddWorldToFavourites(UserDataWorld world);
		bool RemoveWorldFromFavourites(UserDataWorld world);
    }
}