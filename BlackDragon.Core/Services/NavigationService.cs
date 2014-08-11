using System;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using BlackDragon.Core.Entities;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using BlackDragon.Core.IoC;
using System.Threading.Tasks;

namespace BlackDragon.Core
{
	public class NavigationService : INavigationService
	{
        //private readonly IActionTarget _actionTarget;
        //private readonly IWorldService _worldService;
        //private readonly IIndexService _indexService;
        //private readonly IAdventureService _adventureService;

        //private bool _trace = true;

        //public void Trace(string text)
        //{
        //    if (_trace)
        //        Console.WriteLine(text);
        //}

        //public NavigationService(IActionTarget actionTarget, IWorldService worldService, IIndexService indexService, IAdventureService adventureService)
        //{
        //    _actionTarget = actionTarget;
        //    _worldService = worldService;
        //    _indexService = indexService;
        //    _adventureService = adventureService;

        //    _worldService.WorldSelected += (s1, e1) =>
        //    {
        //        _indexService.Clear();
        //        _indexService.Load(_worldService.SelectedWorld);
        //    };
        //    _worldService.Load();

        //    _adventureService.AdventureSelected += (s2, e2) =>
        //    {
        //        var adventure = _adventureService.Adventure;
        //        if (adventure != null)
        //        {
        //            Console.WriteLine("Indexing adventure");
        //            _indexService.ClearForBaseUrl(adventure.ContentPath);
        //            _indexService.Load(adventure);
        //            NavigateTo(_adventureService.CurrentPage());
        //        }
        //    };
        //}

        //public void NavigateTo(UserDataWorld world)
        //{
        //    var ws = DC.Get<IWorldService>();
        //    ws.SetSelectedWorld(world);

        //    var actions = new List<ArchiveAction>();
        //    actions.Add(new ArchiveAction(ActionType.Open, ViewType.AdventureMenu));
        //    if (ws.SelectedWorld.Map != null && !string.IsNullOrEmpty(ws.SelectedWorld.Map.ContentPath))
        //        actions.Add(new ArchiveAction(ActionType.Display, ViewType.MapPage, ws.SelectedWorld.Map.ContentPath));

        //    _actionTarget.ReceiveActions(actions);
        //}

        //public void NavigateTo(Map map)
        //{
        //    if (_actionTarget != null && map != null)
        //    {
        //        var action = new ArchiveAction(ActionType.Display, ViewType.MapPage, map.ContentPath);
        //        _actionTarget.ReceiveAction(action);
        //    }
        //}

        //public void NavigateTo(Page page, bool keepCurrentView = false)
        //{
        //    if (page != null)
        //        Console.WriteLine("Navigating to page " + page.ContentPath);
        //    else
        //        Console.WriteLine("Attempting to navigate to a null page");

        //    var actions = new List<ArchiveAction>();
        //    if (_actionTarget != null && page != null)
        //    {
        //        if (!keepCurrentView)
        //            actions.Add(new ArchiveAction(ActionType.Open, ViewType.Content));

        //        actions.Add(new ArchiveAction(ActionType.Display, ViewType.ContentPage, page.ContentPath));

        //        if (!string.IsNullOrEmpty(page.MapContentPath))
        //            actions.Add(new ArchiveAction(ActionType.Display, ViewType.MapPage, page.MapContentPath));
        //    }

        //    _actionTarget.ReceiveActions(actions);
        //}
		     
        //public void NavigateTo(MapMarker marker)
        //{
        //    var page = _indexService.Get<Page>(marker.ContentPath);
        //    if (page != null)
        //        NavigateTo(page);
        //}

        //public void NavigateToAdventure(string contentPath)
        //{
        //    _adventureService.Load(contentPath);
        //}

        //public void NavigateToWorldMenu()
        //{
        //    var action = new ArchiveAction(ActionType.Open, ViewType.WorldMenu);
        //    SendAction(action);
        //}

        //public void SendAction(ArchiveAction action)
        //{
        //    if (_actionTarget != null)
        //    {
        //        _actionTarget.ReceiveAction(action);
        //    }
        //}

        //public void SendActionFromUrl(string absoluteUrl)
        //{
        //    //Here we can parse the url to determine exactly which action to send
        //    var action = new ArchiveAction(ActionType.Display, ViewType.ContentPage, absoluteUrl);
        //    SendAction(action);
        //}
	}
}

