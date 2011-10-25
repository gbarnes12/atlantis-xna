using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics;
using Xen.Ex.Material;

namespace Atlantis
{
    /// <summary>
    /// Generates a basic terrain without any texturizing just based on a heightmap.
    /// Algorithm based upon: http://mitohnehaare.de/2011/09/07/terrain-101-land-in-sicht/
    /// </summary>
    public class Terrain : IDraw, IContentOwner
    {

        struct CustomVertexPositionColor
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector4 colour;
            public Vector2 texCoord;	//texture coord is unused in this example, but is required by MaterialShader

            //constructor
            public CustomVertexPositionColor(Vector3 position, Vector3 normal, Vector4 colour)
            {
                this.position = position;
                this.normal = normal;
                this.colour = colour;
                this.texCoord = new Vector2();
            }
        }

        private Matrix worldMatrix;
        private Matrix scaleMatrix;
        private Matrix rotationMatrix;

        private Texture2D heightmap;

        private string file;

        private IIndices Indices;
        private IVertices Vertices;
        private IShader Shader;

        public Terrain(ContentRegister content, string file, Vector3 position, float scale)
        {
            this.file = file;

            content.Add(this);

            Matrix.CreateTranslation(ref position, out this.worldMatrix);
            Matrix.CreateScale(scale, out this.scaleMatrix);
        }

        public void LoadContent(ContentState state)
        {
            this.heightmap = state.Load<Texture2D>(file);

            MaterialShader material = new MaterialShader();
            /*material.SpecularColour = Color.LightYellow.ToVector3() * 0.5f;

            Vector3 lightDirection = new Vector3(-1, -1, -1); //a dramatic direction

            //Note: To use vertex colours with a MaterialShader, UseVertexColour has to be set to true
            material.UseVertexColour = true;

            //create a directional light
            MaterialLightCollection lights = new MaterialLightCollection();
            lights.CreateDirectionalLight(-lightDirection, Color.WhiteSmoke);

            material.LightCollection = lights;*/
            material.UseVertexColour = true;
            Shader = material;

            SetupFirstChunk();
        }


        private void SetupFirstChunk()
        {
            Color[] heights = new Color[heightmap.Width * heightmap.Height];
            heightmap.GetData<Color>(heights);

            CustomVertexPositionColor[] Vertices = new CustomVertexPositionColor[128 * 128];

            int index = 0;

            for (int z = 0; z < 128; z++)
            {
                for (int x = 0; x < 128; x++)
                {
                    Vertices[z * 128 + x] = new CustomVertexPositionColor(new Vector3(x * 10.0f, heights[z * 128 + x].R, z * 10.0f),  Vector3.Up, new Vector4(122, 122, 122, 1));

                    //Vertices[z * 128 + x] = new VertexPositionColor(new Vector3(x * 10.0f, heights[z * 128 + x].R, z * 10.0f), Color.Green);
                }
            }

            this.Vertices = new Vertices<CustomVertexPositionColor>(Vertices);

            int[] indices = new int[127 * 127 * 6];
            index = 0;

            for (int z = 0; z < 127; z++)
            {
                for (int x = 0; x < 127; x++)
                {
                    indices[index + 0] = z * 128 + x;
                    indices[index + 1] = indices[index + 0] + 128 + 1;
                    indices[index + 2] = indices[index + 0] + 128;
                    indices[index + 3] = indices[index + 0];
                    indices[index + 4] = indices[index + 0] + 1;
                    indices[index + 5] = indices[index + 0] + 128 + 1;

                    index += 6;
                }
            }

            Indices = new Indices<int>(indices);

            this.Vertices.ResourceUsage = ResourceUsage.Dynamic;
        
        }

        public void Draw(DrawState state)
        {
            using (state.WorldMatrix.Push(ref worldMatrix))
            {
                using (state.WorldMatrix.Push(ref scaleMatrix))
                {
                    if (this.CullTest(state))
                    {
                        //bind the shader
                        using (state.Shader.Push(Shader))
                        {
                            Vertices.Draw(state, Indices, PrimitiveType.TriangleList);
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
