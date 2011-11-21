using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Structures;
using GameApplicationTools.Misc;
using GameApplicationTools;
using SpaceCommander.Scripts.GamePlay;
using GameApplicationTools.Actors.Models;
using GameApplicationTools.UI;
using Microsoft.Xna.Framework;
using GameApplicationTools.Actors.Cameras;
using SpaceCommander.Actors;
using Microsoft.Xna.Framework.Graphics;

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
            WorldManager.Instance.GetActor<SkySphere>("SkySphereSkyGameView").Position = WorldManager.Instance.GetActor<ChaseCamera>("GamePlayCamera").Position;
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff

            Ship ship = new Ship("SpaceShip", ID, Vector3.Zero);
            ship.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(ship);

            Planet planet = new Planet("GamePlanetEarth", ID, new Vector3(-1200, 0, -600), 700f);
            planet.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(planet);

            Planet planet2 = new Planet("GamePlanetEarth2", ID, new Vector3(700, 0, -10300), 400f);
            planet2.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(planet2);

            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                CollisionObject asteroid01 = new CollisionObject("asteroid" + i, GameApplication.Instance.ModelPath + "asteroid", random.Next(20,100), new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300),random.Next(0,360));
                asteroid01.LoadContent(GameApplication.Instance.GetGame().Content);
                WorldManager.Instance.AddActor(asteroid01);
            }
/*
            ThirdPersonCamera tpcamera = new ThirdPersonCamera("GamePlayCamera", new Vector3(0, 100, 300), ship);
            tpcamera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(tpcamera);
            */

            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0,50,600), ship);
            camera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(camera);

            /*
            Camera camera = new Camera("GamePlayCamera", ID, new Vector3(0, 0, 600), Vector3.Zero);
            camera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(camera);
         //   CameraManager.Instance.CurrentCamera = "GamePlayCamera";
            */
            SkySphere sky = new SkySphere("SkySphereSkyGameView", ID, Vector3.Zero, GameApplication.Instance.TexturePath + "space", 10000f);
            sky.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(sky);
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", ID, new Vector2(400, 100), Color.Yellow, "GamePlayView", GameApplication.Instance.GetGame().Content.Load<SpriteFont>(GameApplication.Instance.FontPath + "Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);


            #endregion
        }
    }
}
