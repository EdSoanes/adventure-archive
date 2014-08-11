using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Core.Entities
{
    public class UserDataWorldList
    {
        public UserDataWorldList()
        {
            WorldData = new List<UserDataWorld>();
        }

        public UserDataWorld LastSelected
        {
            get;
            set;
        }

        public List<UserDataWorld> WorldData
        {
            get;
            set;
        }
    }
}
