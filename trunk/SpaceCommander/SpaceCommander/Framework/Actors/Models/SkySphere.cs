namespace GameApplicationTools.Actors.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Interfaces;
    using Cameras;
    using Resources;
    using Misc;

    public class SkySphere : Actor
    {
        #region Private

        private Model model;
        VertexBuffer VertextBuffer;
        IndexBuffer IndexBuffer;
        TextureFilter textureFilter = TextureFilter.Linear;
        TextureMappingEffect effect;
        Texture2D texture;
        String textureFile;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position">center of the skysphere</param>
        /// <param name="textureFile">texture for the skysphere</param>
        /// <param name="scale">scale of the skysphere</param>
        public SkySphere(String ID, String textureFile, float scale)
            : base(ID, null)
        {
            this.Scale = new Vector3(scale, scale, scale);
            this.textureFile = textureFile;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position">center of the skysphere</param>
        /// <param name="textureFile">texture for the skysphere</param>
        /// <param name="scale">scale of the skysphere</param>
        public SkySphere(String ID, String GameViewID, String textureFile, float scale)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(scale, scale, scale);
            this.textureFile = textureFile;
        }


        public override void LoadContent()
        {
            //create a new effect
            effect = new TextureMappingEffect(ResourceManager.Instance.GetResource<Effect>("TextureMappingEffect").Clone());

            //load the sphere; model stays the same for every kind of skysphere
            model = ResourceManager.Instance.GetResource<Model>("sphere");

            //load the model's texture 
            texture = ResourceManager.Instance.GetResource<Texture2D>(textureFile);

            //set texture to the effect
            effect.Texture = texture;

            //set the effect to every part of every mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    part.Effect = effect;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        public void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (model != null)
            {
                //get camera (View & Projection Matrix)
                Camera camera = CameraManager.Instance.GetCurrentCamera();

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);
                
                //set cullMode to None
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.CullClockwiseFace;
                GameApplication.Instance.GetGraphics().RasterizerState = rs;

                GameApplication.Instance.GetGraphics().SamplerStates[0] = new SamplerState()
                {
                    Filter = TextureFilter.Linear,
                    
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap
                };

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (TextureMappingEffect eff in mesh.Effects)
                    {
                        //WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(0.002f, 0.002f, 0.002f));
                        eff.World = AbsoluteTransform;
                        eff.View = camera.View;
                        eff.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

                //reset cullMode
                rs = null;
                rs = new RasterizerState();
                rs.CullMode = CullMode.CullCounterClockwiseFace;
                GameApplication.Instance.GetGraphics().RasterizerState = rs;
            }
        }
    }
}
