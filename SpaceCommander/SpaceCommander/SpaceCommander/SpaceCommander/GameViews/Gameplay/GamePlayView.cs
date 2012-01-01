using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Structures;
using GameApplicationTools.Misc;
using GameApplicationTools;
using SpaceCommander.Scripts.GamePlay;
using GameApplicationTools.Actors.Advanced;
using GameApplicationTools.UI;
using Microsoft.Xna.Framework;
using GameApplicationTools.Actors.Cameras;
using SpaceCommander.Actors;
using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools.Actors.Primitives;
using GameApplicationTools.Input;
using GameApplicationTools.Actors;
using GameApplicationTools.Actors.Properties;
using SpaceCommander.UI;
using SpaceCommander.GameViews.Gameplay.GameLogics;
using GameApplicationTools.Cameras;


namespace SpaceCommander.GameViews.Gameplay
{
    public partial class GamePlayView : GameView
    {
        CollisionGameLogic collision;

        public GamePlayView()
            : base("GamePlay")
        {
            BlocksInput = false;
            BlocksLoading = false;
            BlocksUpdating = false;
            BlocksRendering = false;

            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnCreateEvent);

            //test in fullscreen
        //    /*

            ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.IsFullScreen = false;
            ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferWidth = 1366;
            ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferHeight = 768;
            ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.ApplyChanges();

            //((SpaceCommander)GameApplication.Instance.GetGame()).graphics.IsFullScreen = true;
           // ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferWidth = 1366;
           // ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferHeight = 720;
          //  ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.ApplyChanges();
       //      */
        }

        public override void LoadContent()
        {
            GameApplication.Instance.GetGame().IsMouseVisible = false;
            MouseDevice.Instance.ResetMouseAfterUpdate = false;

            RegisterActors();
            RegisterEvents();
            ScriptManager.Instance.ExecuteScript(GamePlayScript.OnLoadEvent);

           collision = new CollisionGameLogic();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            MouseDevice.Instance.ResetMouseAfterUpdate = false;
            //MouseDevice.Instance.Update();
            WorldManager.Instance.GetActor("skySphere").Position = CameraManager.Instance.GetCurrentCamera().Position; //new Vector3(0,0, WorldManager.Instance.GetActor("SpaceShip").Position.Z);
            ((TextElement)UIManager.Instance.GetActor("TextElementHeadline2")).Text = "Nodes Culled: "+ SceneGraphManager.NodesCulled.ToString();
            ((TextElement)UIManager.Instance.GetActor("TextElementHeadline2")).Text = "MouseDevice Position: " + MouseDevice.Instance.Position.ToString();

            ((TextElement)UIManager.Instance.GetActor("TextPositionShip")).Text = "Ray Position: " + ((Ship)(WorldManager.Instance.GetActor("SpaceShip"))).getRayPosition().ToString();
 

            //enable shooting
            if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.Space))
                ||(MouseDevice.Instance.IsButtonDown(MouseButtons.Left) && MouseDevice.Instance.WasButtonPressed(MouseButtons.Left)))
            {
                ((Laser)WorldManager.Instance.GetActor("testlaser")).Visible = true;
                ((Laser)WorldManager.Instance.GetActor("testlaser")).fire(WorldManager.Instance.GetActor("SpaceShip").Position, ((Ship)WorldManager.Instance.GetActor("SpaceShip")).fireTarget);
            }

             if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.D1)))
             {
                 CameraManager.Instance.CurrentCamera = "chaseCamera";
             }
             else  if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.D2)))
             {
                 CameraManager.Instance.CurrentCamera = "pathCamera";
             }
             else  if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.D3)))
             {
                 CameraManager.Instance.CurrentCamera = "fpsCamera";
             }
            

            //   

          

            base.Update(gameTime);

            // update game logics!
            collision.Update(gameTime, SceneGraphManager);
        }

        public void RegisterEvents()
        {

        }

        public void RegisterActors()
        {
            #region 3D Stuff

            SceneGraphManager.CullingActive = false;

            float testTimeOffset = 6000;

            //ship-position path
            Path path = new Path("targetPath",ID);
            path.LoadContent();
            path.AddPoint(new Vector3(0, -100, 0), 0);
            path.AddPoint(new Vector3(0, -300, -8000), testTimeOffset+2000);
            path.AddPoint(new Vector3(500, -100, -16000), 2*testTimeOffset+4000);
            path.AddPoint(new Vector3(000, 500, -24000), 3*testTimeOffset+6000);
            path.AddPoint(new Vector3(0, 0, -30000), 4*testTimeOffset+8000);

            //camera-position path
            Path path2 = new Path("positionPath", ID,Color.Green);
            path2.LoadContent();
            path2.AddPoint(new Vector3(0, -100, 500), 0);
            path2.AddPoint(new Vector3(0, -300, -8000), testTimeOffset+2400);
            path2.AddPoint(new Vector3(500, -100, -16000), 2*testTimeOffset+4400);
            path2.AddPoint(new Vector3(000, 500, -24000), 3*testTimeOffset+6400);
            path2.AddPoint(new Vector3(0, 0, -29000), 4*testTimeOffset+8000);

            path.SetTangents();
            path2.SetTangents();

            SceneGraphManager.RootNode.Children.Add(path);
            SceneGraphManager.RootNode.Children.Add(path2);

            //create a ship
            Ship ship = new Ship("SpaceShip", ID,path);
            ship.LoadContent();
            ship.Updateable = true;


            Planet p = new Planet("testplanet", 100000);
            p.Position = new Vector3(-1000,-1000,-10000);
            p.Visible = true;
            p.LoadContent();


            SceneGraphManager.RootNode.Children.Add(p);
            

            Laser laser = new Laser("testlaser","laser", 5);
            laser.Visible = false;
            laser.LoadContent();
            SceneGraphManager.RootNode.Children.Add(laser);

            SceneGraphManager.RootNode.Children.Add(ship);

            //cameras
            ChaseCamera chaseCamera = new ChaseCamera("chaseCamera", new Vector3(0, 50, 700), ship);
            chaseCamera.LoadContent();

            FPSCamera fpsCamera = new FPSCamera("fpsCamera", Vector3.Zero, Vector3.UnitZ,500);
            fpsCamera.LoadContent();
          
            PathCamera pathCamera = new PathCamera("pathCamera",path,path2);
            pathCamera.LoadContent();


            CameraManager.Instance.CurrentCamera = "pathCamera";


            //create a skySphere
            SkySphere skySphere = new SkySphere("skySphere", "space", 30000);
            skySphere.LoadContent();
            SceneGraphManager.RootNode.Children.Add(skySphere);
         
            //100 asteroids with random position, scale and rotation
            Random random = new Random();

            for (int i = 0; i < 100; i++)
            {
               // EffectProperty effectProp = new EffectProperty();
                //effectProp.Effect = ResourceManager.Instance.GetResource<Effect>("DefaultEffect");

                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", 1f);
                //asteroid01.Properties.Add(ActorPropertyType.EFFECT, effectProp);

                CollideableProperty collidable = new CollideableProperty();
                asteroid01.Properties.Add(ActorPropertyType.COLLIDEABLE, collidable);

                asteroid01.Scale = new Vector3(random.Next(15, 100), random.Next(15, 100), random.Next(15, 100));
                asteroid01.Rotation = Quaternion.CreateFromYawPitchRoll(random.Next(20,100),random.Next(20,100),random.Next(20,100));
                asteroid01.Position = new Vector3(random.Next(-1000, 1000), random.Next(-500, 500), random.Next(-100, 0) - i * 300);
                asteroid01.LoadContent();

                SceneGraphManager.RootNode.Children.Add(asteroid01);
            }
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", new Vector2(500, 20), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            headline.Visible = false;
            UIManager.Instance.AddActor(headline);

            TextElement TextPositionShip = new TextElement("TextPositionShip", new Vector2(500, 40), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            TextPositionShip.Scale = 1f;
            TextPositionShip.Visible = true;
            UIManager.Instance.AddActor(TextPositionShip);


          
            //create crossfade
            CrossHair crosshair_near = new CrossHair("crosshair_near", "SpaceShip", "crosshair_near", 1500);
            crosshair_near.LoadContent();
            UIManager.Instance.AddActor(crosshair_near);
            #endregion
        }
    }
}
