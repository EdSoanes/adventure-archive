using BlackDragon.Core.IoC;
using System;

namespace BlackDragon.Core
{
    //public enum ActionType
    //{
    //    /// <summary>
    //    /// Opens a view, page or pane
    //    /// </summary>
    //    Open,

    //    /// <summary>
    //    /// Display some content in a view, page or pane
    //    /// </summary>
    //    Display,

    //    /// <summary>
    //    /// Reposition some content in a view, page or pane
    //    /// </summary>
    //    DisplayTo,

    //    /// <summary>
    //    /// Close a view, page or pane
    //    /// </summary>
    //    Close,

    //    /// <summary>
    //    /// Persist some specified data
    //    /// </summary>
    //    Save,

    //    /// <summary>
    //    /// Load some specified data
    //    /// </summary>
    //    Load
    //}

    //public enum ViewType
    //{
    //    WorldMenu,
    //    AdventureMenu,
    //    Content,
    //    ContentPage,
    //    MapPage,
    //    LibraryPage,
    //    NotesPage,
    //    FavouritesPage,
    //    SearchPage,
    //    NotesPopup
    //}

    //public class ArchiveAction
    //{
    //    public ActionType ActionType
    //    {
    //        get;
    //        private set;
    //    }

    //    public ViewType ViewType
    //    {
    //        get;
    //        private set;
    //    }

    //    private string _contentPath = "";
    //    public string ContentPath
    //    {
    //        get { return _contentPath; }
    //        set
    //        {
    //            if (value.ToLower().EndsWith(".aspx"))
    //                _contentPath = value.Substring(0, value.Length - 5);
    //            else
    //                _contentPath = value;
    //        }
    //    }

    //    public string AbsoluteContentPath
    //    {
    //        get 
    //        {
    //            var ws = DC.Get<IWorldService>();
    //            return ws.Domain.CombineUrl(ContentPath); 
    //        }
    //    }

    //    public float ContentX
    //    {
    //        get;
    //        private set;
    //    }

    //    public float ContentY
    //    {
    //        get;
    //        private set;
    //    }

    //    public float ContentZoom
    //    {
    //        get;
    //        private set;
    //    }
		
    //    public bool ShouldPositionContent 
    //    {
    //        get { return ContentX != 0 && ContentY != 0 && ContentZoom != 0; }
    //    }

    //    public object Data
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// Constructor for Display in a View some Content
    //    /// </summary>
    //    /// <param name="actionType">Action type.</param>
    //    /// <param name="viewType">View type.</param>
    //    /// <param name="contentPath">Content path.</param>
    //    public ArchiveAction(ActionType actionType, ViewType viewType, string contentPath)
    //    {
    //        ActionType = actionType;
    //        ViewType = viewType;
    //        ContentPath = contentPath;
    //    }

    //    /// <summary>
    //    /// Constructor for Display/DisplayTo in a View some Content at a specified Position
    //    /// </summary>
    //    /// <param name="actionType">Action type.</param>
    //    /// <param name="viewType">View type.</param>
    //    /// <param name="contentPath">Content path.</param>
    //    /// <param name="contentX">Content x.</param>
    //    /// <param name="contentY">Content y.</param>
    //    /// <param name="contentZoom">Content zoom.</param>
    //    public ArchiveAction(ActionType actionType, ViewType viewType, string contentPath, float contentX, float contentY, float contentZoom)
    //        : this(actionType, viewType, contentPath)
    //    {
    //        ContentX = contentX;
    //        ContentY = contentY;
    //        ContentZoom = contentZoom;
    //    }

    //    /// <summary>
    //    /// Constructor for Save/Load some Data
    //    /// </summary>
    //    /// <param name="actionType">Action type.</param>
    //    /// <param name="data">Data.</param>
    //    public ArchiveAction(ActionType actionType, object data)
    //    {
    //        ActionType = actionType;
    //        Data = data;
    //    }

    //    /// <summary>
    //    /// Constructor to Open/Close a View
    //    /// </summary>
    //    /// <param name="actionType">Action type.</param>
    //    /// <param name="viewType">View type.</param>
    //    public ArchiveAction(ActionType actionType, ViewType viewType)
    //    {
    //        ActionType = actionType;
    //        ViewType = viewType;
    //    }
    //}
}

