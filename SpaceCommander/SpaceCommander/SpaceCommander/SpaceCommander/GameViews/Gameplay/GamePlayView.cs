using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Structures;
using GameApplicationTools.Misc;
using GameApplicationTools;
using SpaceCommander.Scripts.GamePlay;

namespace SpaceCommander.GameViews.Gameplay
{
    public partial class GamePlayView : GameView, IGameView
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


        public GamePlayView()
            : base("GamePlay")
        {
            BlocksInput = false;
            BlocksLoading = false;
            BlocksUpdating = false;
            BlocksRendering = false;

            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnCreateEvent);
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnLoadEvent);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
          
        }
    }
}
