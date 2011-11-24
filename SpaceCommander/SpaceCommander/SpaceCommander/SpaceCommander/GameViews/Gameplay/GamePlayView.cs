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
using GameApplicationTools.Actors.Primitives;

namespace SpaceCommander.GameViews.Gameplay
{
    public partial class GamePlayView : GameView, IGameView
    {
        public GamePlayView()
            : base("GamePlay")
        {
            BlocksInput = false;
            BlocksLoading = false;
            BlocksUpdating = false;
            BlocksRendering = false;

            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnCreateEvent);
        }

        public override void LoadContent()
        {
            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnLoadEvent);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
           // WorldManager.Instance.GetActor<SkySphere>("SkySphereSkyGameView").Position = WorldManager.Instance.GetActor<ChaseCamera>("GamePlayCamera").Position;
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff
            /*
            Ship ship = new Ship("SpaceShip", ID, Vector3.Zero);
            ship.LoadContent();
            WorldManager.Instance.AddActor(ship);

            Planet planet = new Planet("GamePlanetEarth", ID, new Vector3(-1200, 0, -600), 700f);
            planet.LoadContent();
            WorldManager.Instance.AddActor(planet);

            Planet planet2 = new Planet("GamePlanetEarth2", ID, new Vector3(700, 0, -10300), 400f);
            planet2.LoadContent();
            WorldManager.Instance.AddActor(planet2);

            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", random.Next(20,100), new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300),random.Next(0,360));
                asteroid01.LoadContent();
                WorldManager.Instance.AddActor(asteroid01);

                //test
                Sphere sphere = new Sphere("testCollSphere"+i, ID, asteroid01.Position, asteroid01.Scale*1.5f);
                sphere.LoadContent();
                WorldManager.Instance.AddActor(sphere);
            }

            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0,50,600), ship);
            camera.LoadContent();
            WorldManager.Instance.AddActor(camera);

            SkySphere sky = new SkySphere("SkySphereSkyGameView", ID, Vector3.Zero, "space", 10000f);
            sky.LoadContent();
            WorldManager.Instance.AddActor(sky);
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", ID, new Vector2(400, 100), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);
            */

            #endregion
        }
    }
}
