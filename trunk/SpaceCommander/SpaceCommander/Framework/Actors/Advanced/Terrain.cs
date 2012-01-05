namespace GameApplicationTools.Actors.Advanced
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Resources;
    using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools.Actors.Cameras;

    public class Terrain : Actor
    {
        #region Private
        TextureMappingEffect effect;
        Texture2D DiffuseTexture;
        Texture2D HeightMap;
        String HeightMapFile;
        String DiffuseMapFile;
        BoundingSphere modelSphere;

        IndexBuffer indexBuffer;
        VertexBuffer vertexBuffer;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="heightMap">heightMap for the Terrain</param>
        /// <param name="heightMap">diffuseMap for the Terrain</param>
        /// <param name="scale">scale of the skysphere</param>
        public Terrain(String ID, String heightMap, String diffuseMap ,float scale)
            : base(ID, null)
        {
            this.Scale = new Vector3(scale, scale, scale);
            this.HeightMapFile = heightMap;
            this.DiffuseMapFile = diffuseMap;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="heightMap">heightMap for the Terrain</param>
        /// /// <param name="heightMap">diffuseMap for the Terrain</param>
        /// <param name="scale">scale of the skysphere</param>
        public Terrain(String ID, String GameViewID, String heightMap, String diffuseMap, float scale)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(scale, scale, scale);
            this.HeightMapFile = heightMap;
            this.DiffuseMapFile = diffuseMap;
        }

        /// <summary>
        /// The method which loads our necessary content from
        /// the resource manager.
        /// </summary>
        public override void LoadContent()
        {
            //create a new effect
            effect = new TextureMappingEffect(ResourceManager.Instance.GetResource<Effect>("TextureMappingEffect").Clone());

            //load the model's texture 
            DiffuseTexture = ResourceManager.Instance.GetResource<Texture2D>(DiffuseMapFile);
            HeightMap = ResourceManager.Instance.GetResource<Texture2D>(HeightMapFile);

            //set texture to the effect
            effect.Texture = DiffuseTexture;

            SetupFirstChunk();
        }

        private void SetupFirstChunk()
        {
            Color[] heights = new Color[HeightMap.Width * HeightMap.Height];
            HeightMap.GetData<Color>(heights);

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topCenter = new Vector2(0.5f, 0.0f);
            Vector2 topRight = new Vector2(1.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 bottomRight = new Vector2(1.0f, 1.0f);

            VertexPositionTexture[] vertices = new VertexPositionTexture[128 * 128];
            int index = 0;

            for (int z = 0; z < 128; z++)
            {
                for (int x = 0; x < 128; x++)
                {
                    vertices[z * 128 + x] = new VertexPositionTexture(new Vector3(x * 10.0f, heights[z * 128 + x].R, z * 10.0f), topLeft);
                }
            }

            this.vertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), VertexPositionTexture.VertexDeclaration, 128 * 128, BufferUsage.WriteOnly);
            this.vertexBuffer.SetData<VertexPositionTexture>(vertices);

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

            this.indexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.ThirtyTwoBits, 127 * 127 * 6, BufferUsage.WriteOnly);
            this.indexBuffer.SetData<int>(indices);
        }

        public override void PreRender()
        {

            //set cullMode to None
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.WireFrame;
            GameApplication.Instance.GetGraphics().RasterizerState = rs;

            base.PreRender();
        }

        public override void Render(SceneGraphManager sceneGraph)
        {
            //get camera (View & Projection Matrix)
            Camera camera = CameraManager.Instance.GetCurrentCamera();


            //WorldMatrix = transforms[mesh.ParentBone.Index] * Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(0.002f, 0.002f, 0.002f));
            effect.World = AbsoluteTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.CurrentTechnique.Passes[0].Apply();   

            GameApplication.Instance.GetGraphics().SetVertexBuffer(this.vertexBuffer);
	        GameApplication.Instance.GetGraphics().Indices = indexBuffer;

            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 128 * 128 * 6, 0, (127 * 127 * 6) / 3);

            //reset cullMode
            RasterizerState rs = new RasterizerState();
            rs = null;
            rs = new RasterizerState();
            rs.FillMode = FillMode.Solid;
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GameApplication.Instance.GetGraphics().RasterizerState = rs;

            base.Render(sceneGraph);
        }
    }
}
