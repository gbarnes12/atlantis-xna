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
    public partial class GamePlayView : GameView
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
           WorldManager.Instance.GetActor<SkySphere>("SkySphereSkyGameView").Position = CameraManager.Instance.GetCurrentCamera().Position;
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff
            Ship ship = new Ship("SpaceShip", ID);
            ship.LoadContent();
            SceneGraphManager.RootNode.Children.Add(ship);

            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0, 50, 600), ship);
            camera.LoadContent();
            CameraManager.Instance.CurrentCamera = "GamePlayCamera";

            Planet planet = new Planet("GamePlanetEarth2", ID, 700f);
            planet.Position = new Vector3(-1200, 0, -600);
            planet.LoadContent();
            SceneGraphManager.RootNode.Children.Add(planet);

            Planet planet2 = new Planet("GamePlanetEarth3", ID, 400f);
            planet2.Position = new Vector3(700, 0, -10300);
            planet2.LoadContent();
            SceneGraphManager.RootNode.Children.Add(planet2);

            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", random.Next(0,360));
                asteroid01.Rotation = Quaternion.CreateFromYawPitchRoll(random.Next(20,100),random.Next(20,100),random.Next(20,100));
                asteroid01.Position = new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300);
                asteroid01.LoadContent();
                
                //test
                Sphere sphere = new Sphere("testCollSphere"+i, ID, 1f);
                sphere.Scale = asteroid01.Scale * 1.5f;
                sphere.Offset = Vector3.Zero;
                sphere.LoadContent();
                asteroid01.Children.Add(sphere);

                SceneGraphManager.RootNode.Children.Add(asteroid01);
            }

            SkySphere sky = new SkySphere("SkySphereSkyGameView", "space", 10000f);
            sky.Position = Vector3.Zero;
            sky.LoadContent();
            SceneGraphManager.RootNode.Children.Add(sky);
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", new Vector2(400, 100), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            UIManager.Instance.AddActor(headline);
            #endregion
        }
    }
}
