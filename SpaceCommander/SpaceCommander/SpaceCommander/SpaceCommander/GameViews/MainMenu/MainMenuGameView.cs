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
    using GameApplicationTools.UI;
    using Scripts.MainMenu;
    using GameApplicationTools.Events;
    using Scripts.GamePlay;
    using Actors;

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

            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnCreateEvent);
        }

        public void LoadContent()
        {
            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnLoadEvent);
        }

        public void Update(GameTime gameTime)
        {
            
        }

         public void RegisterEvents()
        {
            this.RegisterEvent(ID, EventType.ButtonEvent_OnClick, "onClickButtonEvent");
        }

        public void RegisterActors()
        {
            #region 3D Stuff
            
            Camera camera = new Camera("mainMenuCamera", ID, new Vector3(0, 0, 1000), Vector3.Zero);
            camera.LoadContent();
            WorldManager.Instance.AddActor(camera);
            CameraManager.Instance.CurrentCamera = "mainMenuCamera";

            Planet planet = new Planet("PlanetEarth", ID, new Vector3(-700, 0, 0), 400f);
            planet.LoadContent();
            WorldManager.Instance.AddActor(planet);

          

            SkySphere sky = new SkySphere("SkySphereSky", ID, Vector3.Zero, "space", 10000f);
            sky.LoadContent();
            WorldManager.Instance.AddActor(sky);
 
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline", ID, new Vector2(400, 100), Color.Yellow, "Space Commander", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);

            Button startNewGameButton = new Button("ButtonStartNewGame", ID, new Vector2(400, 150), ResourceManager.Instance.GetResource<Texture2D>("startnewgame_button"), 312, 83);
            startNewGameButton.IsVisible = true;
            startNewGameButton.IsUpdateable = true;
            startNewGameButton.LoadContent();
            WorldManager.Instance.AddActor(startNewGameButton);

            #endregion
        }

        public bool onClickButtonEvent(Event Event)
        {
            if (((Button)((ButtonEvent_OnClick)Event).Sender).ID == "ButtonStartNewGame")
            {
                GameConsole.Instance.WriteLine("Button was clicked");
                IGameView gamplayView = GameViewManager.Instance.GetGameView("GamePlay") as IGameView;
                gamplayView.BlocksInput = false;
                gamplayView.BlocksLoading = false;
                gamplayView.BlocksRendering = false;
                gamplayView.BlocksUpdating = false;

                BlocksInput = true;
                BlocksLoading = true;
                BlocksRendering = true;
                BlocksUpdating = true;

                ScriptManager.Instance.ExecuteScript(GamePlayScript.OnCreateEvent);
                ScriptManager.Instance.ExecuteScript(GamePlayScript.OnLoadEvent);
            }

            return true;
        }
    }
}
