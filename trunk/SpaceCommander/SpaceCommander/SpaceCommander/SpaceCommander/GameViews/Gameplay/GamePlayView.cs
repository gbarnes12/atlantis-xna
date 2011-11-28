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
using GameApplicationTools.Input;

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
            GameApplication.Instance.GetGame().IsMouseVisible = true;
            MouseDevice.Instance.ResetMouseAfterUpdate = false;

            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnLoadEvent);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            WorldManager.Instance.GetActor("skySphere").Position = WorldManager.Instance.GetActor("SpaceShip").Position;
            ((TextElement)UIManager.Instance.GetActor("TextElementHeadline2")).Text = SceneGraphManager.NodesCulled.ToString();
           
            
            base.Update(gameTime);
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff
            Ship ship = new Ship("SpaceShip", ID);
            ship.LoadContent();
            ship.Updateable = true;
            SceneGraphManager.RootNode.Children.Add(ship);

            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0, 50, 700), ship);
            camera.LoadContent();
            CameraManager.Instance.CurrentCamera = "GamePlayCamera";

       //     SceneGraphManager.RootNode.Children.Add(WorldManager.Instance.GetActor("GamePlanetEarth"));

            Planet planet2 = new Planet("GamePlanetEarth3", ID, 400f);
            planet2.Position = new Vector3(700, 0, -10300);
            planet2.LoadContent();
            SceneGraphManager.RootNode.Children.Add(planet2);

            SkySphere skySphere = new SkySphere("skySphere", "space", 100000);
            skySphere.LoadContent();
            SceneGraphManager.RootNode.Children.Add(skySphere);
         
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", random.Next(0,360));
                asteroid01.Rotation = Quaternion.CreateFromYawPitchRoll(random.Next(20,100),random.Next(20,100),random.Next(20,100));
                asteroid01.Position = new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300);
                asteroid01.Scale = new Vector3(10f,10f,10f);
                asteroid01.LoadContent();
                
                //test
                Sphere sphere = new Sphere("testCollSphere"+i, ID, 1f);
                sphere.Scale = asteroid01.Scale * 1.5f;
                sphere.Offset = Vector3.Zero;
                sphere.LoadContent();
                asteroid01.Children.Add(sphere);

                SceneGraphManager.RootNode.Children.Add(asteroid01);
            }

            //SceneGraphManager.RootNode.Children.Add(WorldManager.Instance.GetActor("SkySphereMainMenuView"));
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", new Vector2(400, 100), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            headline.Visible = false;
            UIManager.Instance.AddActor(headline);
            #endregion
        }
    }
}
