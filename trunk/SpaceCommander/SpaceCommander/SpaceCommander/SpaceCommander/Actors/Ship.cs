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
using Microsoft.Xna.Framework.Input;
using GameApplicationTools.UI;
using GameApplicationTools.Cameras;

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

        private float offsetAcceleration = 9;

        //velocity right-|Up- direction. allows the player to move the ship while still following the path
        private Vector3 offsetVelocity = Vector3.Zero;

        //stores the current offset position caused by offsetVelocity
        private Vector3 offsetPosition;

        private BoundingSphere modelSphere;

        Sphere sphere;

        Vector3 right;

        Vector3 tempVelocity = Vector3.Zero;

        Path path;

        Ray ray;

        double time = 0;

        #endregion

        #region Public

        public Vector3 fireTarget;

        public BoundingSphere Sphere { get; set; }

        #endregion

        public Ship(String ID, String GameViewID)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            sphere = new Sphere(ID + "_sphere", 1f);
            this.Children.Add(sphere);
        }

        public Ship(String ID, String GameViewID,Path path)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            this.Scale = new Vector3(50,50,50);
            sphere = new Sphere(ID + "_sphere", 1f);
            this.path = path;
            this.Children.Add(sphere);
        }


        public override void LoadContent()
        {
            //load standard model
            model = ResourceManager.Instance.GetResource<Model>("spaceship");
            sphere.LoadContent();
            CalculateBoundingSphere();

            base.LoadContent();
        }

        public override void Update(SceneGraphManager sceneGraph)
        {
            //get mouse 3d-ray
            ray = CameraManager.Instance.GetCurrentCamera().GetMouseRay(MouseDevice.Instance.Position);
           
            //update time
            time += sceneGraph.GameTime.ElapsedGameTime.TotalMilliseconds;

            //calculate fireTarget (farPlane = max distance)
            fireTarget = ray.Position - ray.Direction * GameApplication.Instance.FarPlane ;

            //get current point on path
            Vector3 pathPosition = path.GetPointOnCurve((float)time);

            //get next point on curve to calculate the current direction
            Vector3 nextPathPosition = path.GetPointOnCurve((float)time+1);

            //calculate direction
            Vector3 currentDirection = nextPathPosition - pathPosition;

            //normalize direction
            currentDirection.Normalize();

            //calculate rotation "caused" by following the path
            yaw = (float)(Math.Atan2(currentDirection.X, currentDirection.Z));
            roll = (float)(Math.Atan2(-currentDirection.Y, currentDirection.Z));    // !!!! TODO

            //set rotation matrix
            Rotation = Quaternion.CreateFromYawPitchRoll(((float)yaw), (float)Math.PI+roll, pitch);

            //calculate right vector note: up vector stays the same every time
            this.right = Vector3.Cross(currentDirection, Vector3.Up);

            //reset offsetVelocity
            offsetVelocity = Vector3.Zero;

            //movement through keyboard input
            if (KeyboardDevice.Instance.IsKeyDown(Keys.A))
            {
                offsetVelocity.X = -offsetAcceleration;
            }
            else if (KeyboardDevice.Instance.IsKeyDown(Keys.D))
            {
                offsetVelocity.X = offsetAcceleration;
            }
            if (KeyboardDevice.Instance.IsKeyDown(Keys.S))
            {
                offsetVelocity.Y = -offsetAcceleration;
            }
            else if (KeyboardDevice.Instance.IsKeyDown(Keys.W))
            {
                offsetVelocity.Y = offsetAcceleration;
            }

            //update offsetPosition (for now just for movement along the x- and y- axis)
            offsetPosition += right * offsetVelocity.X + Vector3.Up * offsetVelocity.Y;

            //movement by mouse

         //   this.tempVelocity.X += 1000* MouseDevice.Instance.Delta.X / ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferWidth;
         //   this.tempVelocity.Y += -1000 * MouseDevice.Instance.Delta.Y / ((SpaceCommander)GameApplication.Instance.GetGame()).graphics.PreferredBackBufferHeight;
        //    this.tempVelocity.Z = velocity.Z;

            //velocity = Vector3.Lerp(velocity, tempVelocity,0.03f);
            //tempVelocity -= velocity;

            //move ship
           // this.Position += velocity;
            this.Position = path.GetPointOnCurve((float)time) + offsetPosition;

            base.Update(sceneGraph);
        }

        //get time used for moving along the path
        public double getTime()
        {
            return this.time;
        }

        public Vector3 getRayPosition()
        {
            return this.ray.Position;
        }



        /*
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
        */


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

                GameApplication.Instance.GetGraphics().BlendState = BlendState.AlphaBlend;

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.LightingEnabled = true;
                        effect.VertexColorEnabled = true;
                        
                        effect.PreferPerPixelLighting = true;
                        effect.World =  transforms[mesh.ParentBone.Index] *
                                                       AbsoluteTransform;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                }
                GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            }
            base.Render(sceneGraph);
        }

       

    }
}