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
            {
                ((Laser)WorldManager.Instance.GetActor("testlaser")).Visible = true;
                ((Laser)WorldManager.Instance.GetActor("testlaser")).fire(WorldManager.Instance.GetActor("SpaceShip").Position);

            //check collision
            foreach (Actor actor in WorldManager.Instance.GetActors().Values)
            {
                if (actor.Properties.ContainsKey(ActorPropertyType.COLLIDEABLE))
                {
                    if (actor == WorldManager.Instance.GetActor("testlaser"))
                        continue;

                    if (actor == WorldManager.Instance.GetActor("SpaceShip"))
                        continue;

                    BoundingSphere transformedActorSphere = new BoundingSphere();

                    transformedActorSphere.Center = Vector3.Transform(actor.BoundingSphere.Center, actor.AbsoluteTransform);
                    transformedActorSphere.Radius = actor.BoundingSphere.Radius;



                    BoundingSphere transformedLaserSphere = new BoundingSphere();

                    transformedLaserSphere.Center = Vector3.Transform(WorldManager.Instance.GetActor("testlaser").BoundingSphere.Center, WorldManager.Instance.GetActor("testlaser").AbsoluteTransform);
                    transformedLaserSphere.Radius = WorldManager.Instance.GetActor("testlaser").BoundingSphere.Radius;


                    if (transformedLaserSphere.Intersects(transformedActorSphere))
                    {
                        // WorldManager.Instance.GetActor("testlaser").Visible = false;
                        //  WorldManager.Instance.GetActor("testlaser").Updateable = false;

                        actor.Visible = false;
                        actor.Updateable = false;

                        break;
                    }
                }
            }
            }

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
            laser.Visible = false;
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
               // EffectProperty effectProp = new EffectProperty();
                //effectProp.Effect = ResourceManager.Instance.GetResource<Effect>("DefaultEffect");

                MeshObject asteroid01 = new MeshObject("asteroid" + i, "asteroid", 1f);
                //asteroid01.Properties.Add(ActorPropertyType.EFFECT, effectProp);
                asteroid01.Scale = new Vector3(random.Next(10, 100), random.Next(10, 100), random.Next(10, 100));
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
