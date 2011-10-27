using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xen.Ex.Graphics.Content;
using Xen;
using Microsoft.Xna.Framework;
using Xen.Ex.Graphics;
using Xen.Graphics;
using Xen.Ex.Material;

namespace Atlantis
{
    class Skydome : IDraw, IContentOwner
    {
        private ModelInstance model;

        private Matrix worldMatrix;
        private Matrix scaleMatrix;

        private IShader shader;


        private Texture2D texture;

        public Skydome(ContentRegister content, Vector3 position, float scale)
        {
            model = new ModelInstance();

            Matrix.CreateTranslation(ref position, out worldMatrix);
            Matrix.CreateScale(scale, out scaleMatrix);

           

            content.Add(this);
        }

        public void LoadContent(ContentState state)
        {
            this.texture = state.Load<Texture2D>("skydome/cloudMap");
            this.model.ModelData = state.Load<ModelData>("skydome/dome");


            MaterialShader material = new MaterialShader();

            material.SpecularColour = new Vector3(1, 1, 1);
            material.DiffuseColour = new Vector3(0.6f, 0.6f, 0.6f);
            material.SpecularPower = 64;

            MaterialTextures textures = new MaterialTextures();
            textures.TextureMap = state.Load<Texture2D>("skydome/cloudMap");
            textures.TextureMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            textures.EmissiveTextureMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            textures.NormalMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            
            material.Textures = textures;

            this.shader = material;
        }

        public void Draw(DrawState state)
        {

            using (state.WorldMatrix.Push(ref worldMatrix))
            {
                using (state.WorldMatrix.PushMultiply(ref scaleMatrix))
                {
                    if (model.CullTest(state))
                    {
                        using (state.Shader.Push(this.shader))
                        {
                            model.Draw(state);
                        }
                    }
                }
            }
        }

        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}