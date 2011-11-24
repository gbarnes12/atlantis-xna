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

    public class MainMenuGameView : GameView
    {
        public MainMenuGameView()
            : base("MainMenu")
        {
            BlocksInput = false;
            BlocksLoading = false;
            BlocksUpdating = false;
            BlocksRendering = false;

            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnCreateEvent);
        }

        public override void LoadContent()
        {
            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(MainMenuScript.OnLoadEvent);
        }

        public void RegisterEvents()
        {
            this.RegisterEvent(ID, EventType.ButtonEvent_OnClick, "onClickButtonEvent");
        }

        public void RegisterActors()
        {
            #region 3D Stuff
            
            Camera camera = new Camera("mainMenuCamera", new Vector3(0, 0, 5), Vector3.Zero);
            camera.LoadContent();
            CameraManager.Instance.CurrentCamera = "mainMenuCamera";

            Box box = new Box("box", 1f);
            box.Position = new Vector3(0, 0, 0);
            //box.Offset = new Vector3(0, 0, 0);
            box.LoadContent();

            Sphere sphere = new Sphere("sphereBox", 3f);
            sphere.Offset = Vector3.Zero;
            box.Children.Add(sphere);

            SceneGraphManager.RootNode.Children.Add(box);
            #endregion

           /* #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline", ID, new Vector2(400, 100), Color.Yellow, "Space Commander", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);

            Button startNewGameButton = new Button("ButtonStartNewGame", ID, new Vector2(400, 150), ResourceManager.Instance.GetResource<Texture2D>("startnewgame_button"), 312, 83);
            startNewGameButton.IsVisible = true;
            startNewGameButton.IsUpdateable = true;
            startNewGameButton.LoadContent();
            WorldManager.Instance.AddActor(startNewGameButton);

            #endregion*/
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
