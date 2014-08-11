using System;
using BlackDragon.Core.Entities;
using System.Collections.Generic;

namespace BlackDragon.Core
{
    public interface IWorldService
    {
        World SelectedWorld { get; set; }

		Action LoadAction(Action<StatusCode> completionAction = null);
    }
}

