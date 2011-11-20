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

namespace SpaceCommander.Actors
{
    public class Ship  : Actor,IDrawableActor
    {
        public Vector3 Position
        {
            get;
            set;
        }

        public float Angle
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public Matrix WorldMatrix
        {
            get;
            set;
        }

        public Matrix RotationMatrix { get; set; }

        public bool IsVisible
        {
            get;
            set;
        }

        public bool IsUpdateable
        {
            get;
            set;
        }


        #region Private

        private Model model;

        private float yaw, pitch, roll;

        private float rotation_speed = 5;

        #endregion

        public Ship(String ID, String GameViewID, Vector3 position)
            : base(ID, GameViewID)
        {
            IsVisible = true;
            this.Position = Position;
            IsUpdateable = true;
            this.Scale = 0.1f;
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(GameApplication.Instance.ModelPath+"p1_wedge");
        }

        public void Update(GameTime gameTime)
        {
            

            if(KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                yaw += rotation_speed; //degrees
            }
            else if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                yaw -= rotation_speed;
            }
            if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                roll -= rotation_speed;
            }            
            else if (KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
                roll += rotation_speed;
            }

            RotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(roll)) * Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
        }

        public void Render(GameTime gameTime)
        {
            if (model != null)
            {
                Camera camera = WorldManager.Instance.GetActor("camera") as Camera;

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, RotationMatrix, new Vector3(Scale));
                        effect.EnableDefaultLighting();
                        effect.World = WorldMatrix;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                }
            }
        }
    }
}
