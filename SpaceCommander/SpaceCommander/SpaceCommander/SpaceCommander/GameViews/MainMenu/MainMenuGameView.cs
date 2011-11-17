namespace SpaceCommander.GameViews.MainMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    using GameApplicationTools;
    using GameApplicationTools.Structures;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Actors.Primitives;
    using GameApplicationTools.Actors.Models;
    using GameApplicationTools.Misc;

    using Actors;
    using GameApplicationTools.UI;
    using Scripts.MainMenu;

    public partial class MainMenuGameView : GameView, IGameView
    {
        #region Private
        bool _blocksRendering;
        bool _blocksUpdating;
        #endregion

        #region Public
        public bool BlocksRendering
        {
            get 
            {
                return _blocksRendering;
            }
            set
            {
                this.SetActorsVisibility(Utils.ToggleBool(value));
                this._blocksRendering = value;
            }
        }

        public bool BlocksUpdating 
        {  
            get 
            {
                return _blocksUpdating;
            }
            set
            {
                this.SetActorsUpdateable(Utils.ToggleBool(value));
                this._blocksUpdating = value;
            } 
        }

        public bool BlocksInput { get; set; }

        public bool BlocksLoading { get; set; }
        #endregion

        public MainMenuGameView()
            : base("MainMenu")
        {
            BlocksInput = false;
            BlocksLoading = false;
            BlocksUpdating = false;
            BlocksRendering = false;

            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnCreateEvent);
        }

        public void LoadContent(ContentManager content)
        {
            LoadActors();
            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnLoadEvent);
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
