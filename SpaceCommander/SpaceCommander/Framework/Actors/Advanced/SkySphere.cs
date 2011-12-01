namespace GameApplicationTools.Actors.Advanced
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

    /// <summary>
    /// Represents a SkySphere which gets drawn by 
    /// a basic sphere. This can be used to draw a 
    /// beautiful sky or some sort of space. 
    /// 
    /// Author: Dominik Finkbeiner
    /// Version: 1.0
    /// </summary>
    public class SkySphere : Actor
    {
        #region Private

        Model model;
        TextureMappingEffect effect;
        Texture2D texture;
        String textureFile;
        BoundingSphere modelSphere;
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

        /// <summary>
        /// Calculates the bounding sphere which we get from
        /// our model meshes!
        /// </summary>
        private void CalculateBoundingSphere()
        {
            //Calculate the bounding sphere for the entire model

            modelSphere = new BoundingSphere();

            foreach (ModelMesh mesh in model.Meshes)
            {
                modelSphere = Microsoft.Xna.Framework.BoundingSphere.CreateMerged(
                                    modelSphere,
                                    mesh.BoundingSphere);
            }
        }

        /// <summary>
        /// Returns an active bounding sphere for this object!
        /// </summary>
        /// <returns>BoundingSphere which gets computed after every update</returns>
        public override BoundingSphere GetBoundingSphere()
        {
            return modelSphere;
        }

        /// <summary>
        /// The method which loads our necessary content from
        /// the resource manager.
        /// </summary>
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

        /// <summary>
        /// Set the necessary render states so we can 
        /// make sure everything draws correctly!
        /// </summary>
        public override void PreRender()
        {
            if (model != null)
            {
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
            }
            base.PreRender();
        }

        /// <summary>
        /// Render the sky sphere with all its transformations and 
        /// effects.
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            if (model != null)
            {
                //get camera (View & Projection Matrix)
                Camera camera = CameraManager.Instance.GetCurrentCamera();

                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);
                
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (TextureMappingEffect eff in mesh.Effects)
                    {
                        //WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(0.002f, 0.002f, 0.002f));
                        eff.World = transforms[mesh.ParentBone.Index] * 
                                         AbsoluteTransform;
                        eff.View = camera.View;
                        eff.Projection = camera.Projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

                //reset cullMode
                RasterizerState rs = new RasterizerState();
                rs = null;
                rs = new RasterizerState();
                rs.CullMode = CullMode.CullCounterClockwiseFace;
                GameApplication.Instance.GetGraphics().RasterizerState = rs;
            }

            base.Render(sceneGraph);
        }
    }
}
