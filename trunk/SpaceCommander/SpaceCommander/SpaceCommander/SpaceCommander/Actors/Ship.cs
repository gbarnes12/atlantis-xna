using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Interfaces;
using GameApplicationTools.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools.Actors.Cameras;
using GameApplicationTools;
using GameApplicationTools.Misc;
using GameApplicationTools.Input;
using GameApplicationTools.Actors.Primitives;

namespace SpaceCommander.Actors
{
    public class Ship : Actor, ICollideable
    {
        #region Private
        private Model model;
        private float yaw, pitch, roll;
        private float max_yaw = 30;
        private float max_roll = 30;
        private float rotation_speed = 45;
        private Vector3 velocity = Vector3.Zero;
        private float speed = 5;
        private BoundingSphere modelSphere;
        public BoundingSphere Sphere { get; set; }

        Sphere sphere;
        #endregion

        public Ship(String ID, String GameViewID)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere = new Sphere(ID + "_sphere", 1f);
            this.Children.Add(sphere);
        }

        public override void LoadContent()
        {
            model = ResourceManager.Instance.GetResource<Model>("p1_wedge");
            sphere.LoadContent();
            CalculateBoundingSphere();

            base.LoadContent();
        }

        public override void Update(SceneGraphManager sceneGraph)
        {
            float speed_x = 0, speed_y = 0,speed_factor =50;

            if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                if (yaw < max_yaw)   
                    if(yaw <0)
                        yaw += 2*rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f; //degrees
                    else
                        yaw += rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f; //degrees
                   
       
                //move vertical
                speed_x = speed_factor * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            }
            else if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                if (yaw > -max_yaw)  
                     if (yaw > 0)
                        yaw -= 2 * rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f; //degrees
                     else
                        yaw -= rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f; //degrees
                   
                //move vertical
                speed_x = -speed_factor * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            }

            //no key is pressed, so slow down rotation
            else if (yaw < 0)
                yaw += rotation_speed *2* (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            else if (yaw > 0)
                yaw -= rotation_speed *2* (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            if (!KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && !KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                //stop rotation
                if (Math.Abs(yaw) <= 2)
                    yaw = 0;
            }

            if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                if (roll > -max_roll)
                    if(roll>0)
                        roll -= 2*rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                    else
                        roll -= rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

                speed_y = -speed_factor * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            }
            else if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                if (roll < max_roll)
                    if(roll<0)
                        roll += 2 * rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                    else
                        roll += rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                   
                speed_y = speed_factor * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
           
            }
            //no key is pressed, so slow down rotation
            else if (roll < 0)
                roll += rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            else if (roll > 0)
                roll -= rotation_speed * (float)sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            if (!KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) && !KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                //stop rotation
                if (Math.Abs(roll) <= 2)
                    roll = 0;
            }

            //move the ship
            Position += new Vector3(-speed_x, speed_y, -1f) * speed;

            if (Position.X < -350)
                Position = new Vector3(-350,Position.Y,Position.Z);
            else if(Position.X > 350)
                Position = new Vector3(350, Position.Y, Position.Z);
            

            //Matrix.CreateRotationX(MathHelper.ToRadians(roll)) * Matrix.CreateRotationY(MathHelper.ToRadians(yaw)) * Matrix.CreateRotationZ(0.0f);
            Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(roll), MathHelper.ToRadians(pitch));


            base.Update(sceneGraph);
        }



        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            modelSphere = new BoundingSphere(Vector3.Zero, 0);

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere transformed = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index]);

                modelSphere = BoundingSphere.CreateMerged(modelSphere, transformed);
            }
        }

        public override BoundingSphere GetBoundingSphere()
        {
            return modelSphere;
        }

        public override void PreRender()
        {
            base.PreRender();
        }

        public override void Render(SceneGraphManager sceneGraph)
        {
            if (model != null)
            {
                Camera camera = CameraManager.Instance.GetCurrentCamera();

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =  transforms[mesh.ParentBone.Index] *
                                                       AbsoluteTransform;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                }
            }
            base.Render(sceneGraph);
        }

       

    }
}