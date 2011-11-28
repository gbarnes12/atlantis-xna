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
            WorldManager.Instance.GetActor("skySphere").Position = new Vector3(0,0, WorldManager.Instance.GetActor("SpaceShip").Position.Z);
            ((TextElement)UIManager.Instance.GetActor("TextElementHeadline2")).Text = SceneGraphManager.NodesCulled.ToString();

            //enable shooting
            if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.Space))
                ((Laser)WorldManager.Instance.GetActor("testlaser")).fire(WorldManager.Instance.GetActor("SpaceShip").Position);
            
            base.Update(gameTime);
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff

            //create a ship
            Ship ship = new Ship("SpaceShip", ID);
            ship.LoadContent();
            ship.Updateable = true;
            SceneGraphManager.RootNode.Children.Add(ship);

            Laser laser = new Laser("testlaser","laser", 5);
            laser.LoadContent();
            SceneGraphManager.RootNode.Children.Add(laser);

            //ceate a chase camera
            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0, 50, 700), ship);
            camera.LoadContent();
            CameraManager.Instance.CurrentCamera = "GamePlayCamera";

            //create a planet
            Planet planet2 = new Planet("GamePlanetEarth3", ID, 400f);
            planet2.Position = new Vector3(700, 0, -10300);
            planet2.LoadContent();
            SceneGraphManager.RootNode.Children.Add(planet2);

            //create a skySphere
            SkySphere skySphere = new SkySphere("skySphere", "space", 100000);
            skySphere.LoadContent();
            SceneGraphManager.RootNode.Children.Add(skySphere);
         
            //100 asteroids with random position, scale and rotation
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", random.Next(0,12));
                asteroid01.Rotation = Quaternion.CreateFromYawPitchRoll(random.Next(20,100),random.Next(20,100),random.Next(20,100));
                asteroid01.Position = new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300);
                asteroid01.LoadContent();

                SceneGraphManager.RootNode.Children.Add(asteroid01);
            }
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
