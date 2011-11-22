namespace GameApplicationTools.Actors.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Misc;
    using Actors.Cameras;
    using Interfaces;

    public class MeshObject : Actor, IDrawableActor, ICollideable
    {
        public Microsoft.Xna.Framework.Vector3 Position
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

        public Microsoft.Xna.Framework.Matrix WorldMatrix
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Matrix RotationMatrix
        {
            get;
            set;
        }

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

        public BoundingSphere Sphere
        {
            get;
            set;
        }

        private String _fileName;

        private Model model;

        public MeshObject(String ID, String modelFile, float scale, Vector3 position)
            : base(ID, null)
        {
            IsVisible = true;
            _fileName = modelFile;
            this.Position = position;
            this.IsUpdateable = true;
            this.Scale = scale;
        }
        public MeshObject(String ID, String modelFile, float scale, Vector3 position,float angle)
            : base(ID, null)
        {
            IsVisible = true;
            _fileName = modelFile;
            this.Position = position;
            this.IsUpdateable = true;
            this.Scale = scale;
            this.Angle = angle;
        }

        public void LoadContent()
        {
            if (_fileName != "")
                model = ResourceManager.Instance.GetResource<Model>(_fileName);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Angle += 1f;
        }

        public void Render(Microsoft.Xna.Framework.GameTime gameTime)
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
                        WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, Matrix.CreateRotationX(MathHelper.ToRadians(Angle)) * Matrix.CreateRotationY(MathHelper.ToRadians(Angle)), new Vector3(Scale));

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
