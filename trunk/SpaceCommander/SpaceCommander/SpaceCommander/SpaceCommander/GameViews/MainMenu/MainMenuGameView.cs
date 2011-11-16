namespace SpaceCommander.GameViews.MainMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    using GameApplicationTools;
    using GameApplicationTools.Structures;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Actors.Primitives;
    using GameApplicationTools.Actors.Models;
    using Microsoft.Xna.Framework.Graphics;

    public class MainMenuGameView : GameView, IGameView
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
        }

        public void LoadContent(ContentManager content)
        {
            FPSCamera camera = new FPSCamera("camera", ID, new Vector3(0, 0, 3), Vector3.Zero);
            camera.LoadContent(content);
            WorldManager.Instance.AddActor(camera);

            Axis axis = new Axis("axis", ID, Vector3.Zero, 1f);
            axis.LoadContent(content);
            WorldManager.Instance.AddActor(axis);

            SkySphere sky = new SkySphere("skysphere", ID, Vector3.Zero, "Textures\\space", 1000f);
            sky.LoadContent(content);
            WorldManager.Instance.AddActor(sky);

           

        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
