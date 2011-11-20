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

    public class SkySphere : Actor, IDrawableActor
    {
        #region Public 

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

        public Microsoft.Xna.Framework.Matrix RotationMatrix { get; set; }

        public bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public bool IsUpdateable { get; set; }

        #endregion

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
        public SkySphere(String ID, Vector3 position,String textureFile, float scale)
            : base(ID, null)
        {
            IsVisible = true;
            IsUpdateable = true;
            this.Position = position;
            this.Scale = scale;
            this.textureFile = textureFile;
            this.Angle = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position">center of the skysphere</param>
        /// <param name="textureFile">texture for the skysphere</param>
        /// <param name="scale">scale of the skysphere</param>
        public SkySphere(String ID, String GameViewID, Vector3 position, String textureFile, float scale)
            : base(ID, GameViewID)
        {
            IsVisible = true;
            IsUpdateable = true;
            this.Position = position;
            this.Scale = scale;
            this.textureFile = textureFile;
        }


        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            //create a new effect
            effect = new TextureMappingEffect(content.Load<Effect>(GameApplication.Instance.EffectPath + "TextureMappingEffect"));

            //load the sphere; model stays the same for every kind of skysphere
            model = content.Load<Model>(GameApplication.Instance.ModelPath + "sphere");

            //load the model's texture 
            texture = content.Load<Texture2D>(textureFile);

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
                Camera camera = WorldManager.Instance.GetActor("camera") as Camera;

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
                        eff.World = Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(0), new Vector3(Scale));
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
