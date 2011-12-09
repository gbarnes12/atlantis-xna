﻿using System;
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
            ((TextElement)UIManager.Instance.GetActor("TextElementHeadline2")).Text = "Nodes Culled: "+ SceneGraphManager.NodesCulled.ToString();

            //enable shooting
            if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                ((Laser)WorldManager.Instance.GetActor("testlaser")).Visible = true;
                ((Laser)WorldManager.Instance.GetActor("testlaser")).fire(WorldManager.Instance.GetActor("SpaceShip").Position);
            }

            base.Update(gameTime);


            //check collision
            foreach (Actor actor in SceneGraphManager.RootNode.Children)
            {
                if (actor.Properties.ContainsKey(ActorPropertyType.COLLIDEABLE) && actor.Visible)
                {
 
                    BoundingSphere transformedSphere = actor.BoundingSphere.Transform(actor.AbsoluteTransform);
                    BoundingSphere transformedLaserSphere = WorldManager.Instance.GetActor("testlaser").BoundingSphere.Transform(WorldManager.Instance.GetActor("testlaser").AbsoluteTransform);


                    if (WorldManager.Instance.GetActor("testlaser").Visible)
                    {
                        if (transformedSphere.Intersects(transformedLaserSphere))
                        {
                            WorldManager.Instance.GetActor("testlaser").Visible = false;
                            GameConsole.Instance.WriteLine("Collision!");

                            actor.Visible = false;
                            actor.Updateable = false;
                        }
                    }
                    
                }
            }
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
            

            Laser laser = new Laser("testlaser","laser", 5);
            laser.Visible = false;
            laser.LoadContent();
            SceneGraphManager.RootNode.Children.Add(laser);

            SceneGraphManager.RootNode.Children.Add(ship);

            //ceate a chase camera
            ChaseCamera camera = new ChaseCamera("GamePlayCamera", new Vector3(0, 50, 700), ship);
            camera.LoadContent();
            CameraManager.Instance.CurrentCamera = "GamePlayCamera";

            //create a planet
            Planet planet2 = new Planet("GamePlanetEarth3", ID, 10000f);
            planet2.Position = new Vector3(10000, 0, -10300);
            planet2.LoadContent();
            SceneGraphManager.RootNode.Children.Add(planet2);
            

            //create a skySphere
            SkySphere skySphere = new SkySphere("skySphere", "space", 50000);
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
            TextElement headline = new TextElement("TextElementHeadline2", new Vector2(600, 20), Color.Yellow, "GamePlayView", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            headline.Scale = 1f;
            headline.Visible = false;
            UIManager.Instance.AddActor(headline);


            //create crossfade
            CrossHair crosshair = new CrossHair("crossHair", "SpaceShip");
            crosshair.LoadContent();
            UIManager.Instance.AddActor(crosshair);
            #endregion
        }
    }
}
