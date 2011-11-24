namespace GameApplicationTools.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Actors;
    using GameApplicationTools.Interfaces;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Misc;

    /// <summary>
    /// A GameView is used to represent the game's 
    /// displays it can be everything from HUD to game play. 
    /// Its used to separate things e.g. menus from game play screens
    /// and so on. 
    /// 
    /// You need to implement the IGameView interface otherwise
    /// it won't work properly. This is just a basic body of the
    /// class.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class GameView : EventListener
    {
        #region Public

        bool _blocksRendering;
        bool _blocksUpdating;

        /// <summary>
        /// This is used to have an unique identifier
        /// for the specific GameView within the game.
        /// </summary>
        public String ID { get; set; }
        public bool BlocksRendering
        {
            get
            {
                return _blocksRendering;
            }
            set
            {
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
                this._blocksUpdating = value;
            }
        }

        public bool BlocksInput { get; set; }

        public bool BlocksLoading { get; set; }
        public SceneGraphManager SceneGraphManager { get; set; }
        #endregion

        public GameView(string ID)
        {
            this.ID = ID;
            SceneGraphManager = new SceneGraphManager();
        }

        public virtual void LoadContent() { }

        /// <summary>
        /// Updates the entire SceneGraphManager on this
        /// game view!
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            if (SceneGraphManager != null)
                SceneGraphManager.Update(gameTime);
        }

        /// <summary>
        /// Renders the entire SceneGraphManager
        /// on this game view!
        /// </summary>
        public virtual void Render()
        {
            if (SceneGraphManager != null)
                SceneGraphManager.Render();
        }

        /// <summary>
        /// Sets every actor which has this
        /// game view id to the current state!
        /// </summary>
        /// <param name="value"></param>
        public void SetActorsVisibility(bool value)
        {
            Dictionary<String, Actor> actors = WorldManager.Instance.GetActors();
            foreach (Actor actor in actors.Values)
            {
                if (actor.GameViewID == ID)
                {
                    actor.Visible = value;
                }
            }
        }


        /// <summary>
        /// Sets every actor which has this
        /// game view id to the current state!
        /// </summary>
        /// <param name="value"></param>
        public void SetActorsUpdateable(bool value)
        {
            Dictionary<String, Actor> actors = WorldManager.Instance.GetActors();
            foreach (Actor actor in actors.Values)
            {
                if (actor.GameViewID == ID)
                {
                    actor.Updateable = value;
                }
            }
        }
    }
}
