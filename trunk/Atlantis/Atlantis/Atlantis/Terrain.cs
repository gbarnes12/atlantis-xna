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


    struct CustomVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texCoord;	//texture coord is unused in this example, but is required by MaterialShader

        //NEW CODE
        //add a vertex colour (which defaults to white)
        public Vector4 colour;


        //constructor
        public CustomVertex(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
            this.texCoord = new Vector2();
            //NEW CODE
            this.colour = Vector4.One;
        }
    }

    class Terrain : IDraw, IContentOwner
    {
        //vertex and index buffers
        private IVertices vertices;
        private IIndices indices;
        private CustomVertex[] vertexData;

        private Texture2D heightmap;
        private string file;
        public Texture2D Texture;
        public Texture2D NormalMap;

        //setup and create the vertices/indices
        public Terrain(ContentRegister content, string file)
        {
            this.file = file;

            content.Add(this);
        }

        public void LoadContent(ContentState state)
        {
            this.heightmap = state.Load<Texture2D>(file);
            this.Texture = state.Load<Texture2D>("grass2");
            
            SetupFirstChunk();
        }

        private void SetupFirstChunk()
        {
            Color[] heights = new Color[heightmap.Width * heightmap.Height];
            heightmap.GetData<Color>(heights);

            vertexData = new CustomVertex[128 * 128];

            int index = 0;

            for (int z = 0; z < 128; z++)
            {
                for (int x = 0; x < 128; x++)
                {
                    vertexData[z * 128 + x] = new CustomVertex(new Vector3(x * 10.0f, heights[z * 128 + x].R, z * 10.0f), Vector3.Up);
                    vertexData[z * 128 + x].colour = Color.Green.ToVector4();
                    vertexData[z * 128 + x].texCoord = new Vector2((float)x / 30.0f, (float)z / 30.0f);

                    //Vertices[z * 128 + x] = new VertexPositionColor(new Vector3(x * 10.0f, heights[z * 128 + x].R, z * 10.0f), Color.Green);
                }
            }

            this.vertices = new Vertices<CustomVertex>(vertexData);

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

            this.indices = new Indices<int>(indices);

            this.vertices.ResourceUsage = ResourceUsage.Dynamic;
        }


        //draw the quad
        public void Draw(DrawState state)
        {
            //draw as usual
            this.vertices.Draw(state, this.indices, PrimitiveType.TriangleList);
        }

        public bool CullTest(ICuller culler)
        {
            //cull test with an bounding box...
            //this time taking into account the z values can also range between -1 and 1
            return true;
        }
    }


    class TerrainDrawer : IDraw
    {
        private Terrain geometry;
        private Matrix worldMatrix;
        private IShader shader;

        public TerrainDrawer(ContentRegister content ,Vector3 position)
        {
            //create the quad
            geometry = new Terrain(content, "chunkheightmap");

            //setup the world matrix
            worldMatrix = Matrix.CreateTranslation(position);

            //create a basic lighting shader with some average looking lighting :-)
            MaterialShader material = new MaterialShader();
            
            //Note: To use vertex colours with a MaterialShader, UseVertexColour has to be set to true
            material.UseVertexColour = false;
            material.SpecularColour = new Vector3(1, 1, 1);
            material.DiffuseColour = new Vector3(0.6f, 0.6f, 0.6f);
            material.SpecularPower = 64;

            MaterialTextures textures = new MaterialTextures();
            textures.TextureMap = geometry.Texture;
            textures.NormalMap = geometry.NormalMap;
            textures.TextureMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            textures.EmissiveTextureMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            textures.NormalMapSampler = TextureSamplerState.AnisotropicHighFiltering;
            material.Textures = textures;

            material.LightCollection = new MaterialLightCollection();
            material.LightCollection.AddLight(material.LightCollection.CreatePointLight(new Vector3(640.0f, 300.0f, 640.0f), 10000f, Color.WhiteSmoke));
            material.LightingDisplayModel = LightingDisplayModel.ForcePerVertex;

            this.shader = material;
           
        }

        public void Draw(DrawState state)
        {
            //push the world matrix, multiplying by the current matrix if there is one
            using (state.WorldMatrix.PushMultiply(ref worldMatrix))
            {
                //cull test the geometry
                if (geometry.CullTest(state))
                {
                    //bind the shader
                    using (state.Shader.Push(shader))
                    {
                        //draw the custom geometry
                        geometry.Draw(state);
                    }
                }
            }
        }

        //always draw.. don't cull yet
        public bool CullTest(ICuller culler)
        {
            return true;
        }
    }
}
