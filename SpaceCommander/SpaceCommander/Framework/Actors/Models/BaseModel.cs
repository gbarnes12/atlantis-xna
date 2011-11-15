namespace Framework.Actors.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Interfaces;
    using Cameras;

    public class BaseModel : Actor, IDrawableActor
    {
        #region Public
        public Model Model { get; set; }

        public Microsoft.Xna.Framework.Vector3 Position { get; set; }

        public float Angle { get; set; }

        public Microsoft.Xna.Framework.Matrix WorldMatrix { get; set; }

        public bool IsVisible { get; set; }

        public float Scale { get; set; }
        #endregion

        String _fileName;

        public BaseModel(String ID, String File, Vector3 Position) 
            : base(ID)
        {
            IsVisible = true;
            _fileName = File;
            this.Position = Position;
        }

        

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            if(_fileName != "")
                Model = content.Load<Model>(_fileName);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Angle += 0.005f;
        }

        public void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(Model != null)
            {
                FPSCamera camera = WorldManager.Instance.GetActor("camera") as FPSCamera;

                Matrix[] transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(0.002f, 0.002f, 0.002f));

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
