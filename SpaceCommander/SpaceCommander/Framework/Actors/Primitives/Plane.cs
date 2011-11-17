namespace GameApplicationTools.Actors.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    using Cameras;
    using Interfaces;
    using Resources;
    using Misc;

    /// <summary>
    /// This will create a plane in the size of 
    /// about 2000 which represents about 2000 meters. 
    /// Besides it will load a basic texture "Kachel2_bump". 
    /// You can use this to have a character running on it 
    /// or show some nice shadowing. 
    /// 
    /// Attention: There is no physics applied to it yet 
    /// which will need to be done in and upcoming release.
    /// Besides we still have some problems regarding the UV
    /// mapping of the texture. It doesn't look as it is supposed
    /// to be. 
    /// 
    /// It inherits from the Actor class and IDrawableActor interface.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Plane : Actor, IDrawableActor
    {

        #region Private
        // those classes are needed in order
        // to create a working plane
        TextureMappingEffect effect;
        VertexBuffer VertexBuffer;
        IndexBuffer IndexBuffer;
        TextureFilter textureFilter = TextureFilter.Linear;
        #endregion

        #region Public
        /// <summary>
        /// The Position of this Actor 
        /// in the World. 
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Sets the current angle of this 
        /// Actor.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Sets the scale of our model
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// The world matrix of the inheriting actor
        /// that we need to set the current scale, position
        /// and rotation.
        /// </summary>
        public Matrix WorldMatrix { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets drawn or not
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public bool IsUpdateable { get; set; }
        #endregion

        public Plane(String ID, Vector3 Position)
            : base(ID, null)
        {
            this.Position = Position;
            this.Angle = 0f;
            this.Scale = 1f;
            this.IsVisible = true;
            this.IsUpdateable = true;

            WorldMatrix = Matrix.Identity;

        }

        public Plane(String ID, String GameViewID, Vector3 Position)
            : base(ID, GameViewID)
        {
            this.Position = Position;
            this.Angle = 0f;
            this.Scale = 1f;
            this.IsVisible = true;
            this.IsUpdateable = true;

            WorldMatrix = Matrix.Identity;

        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            FPSCamera cam = WorldManager.Instance.GetActor<FPSCamera>("camera");

            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topRight = new Vector2(100f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 100f);
            Vector2 bottomRight = new Vector2(100f, 100f);

            // since we use an index buffer we just need to declare
            // four vertices thus we can create a quad only streches to the z-Axis
            // and of course x-Axis. 
            VertexPositionTexture[] vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(2000f, 0f, -2000f), topRight),
                new VertexPositionTexture(new Vector3(2000f, 0f, 2000f), bottomRight),
                new VertexPositionTexture(new Vector3(0f, 0f, 2000f), bottomLeft),
                new VertexPositionTexture(new Vector3(0f, 0f, -2000f), topLeft),
            };

            // Set the index buffer for each vertex, using
            // clockwise winding
            short[] indices = new short[] { 0, 1, 2, 0, 2, 3 };

            VertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            IndexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

            VertexBuffer.SetData<VertexPositionTexture>(vertices);
            IndexBuffer.SetData<short>(indices);
            vertices = null;
            indices = null;

            // now we need to load our texture mapping effect and of course our texture into cache
            // this may need to be redesigned once we use some sort of resource manager!
            effect = new TextureMappingEffect(content.Load<Effect>("Effects\\TextureMappingEffect"));
            effect.Texture = content.Load<Texture2D>("Textures\\Kachel2_bump");

        }

        /// <summary>
        /// The Update method. This will
        /// take care of updating our world matrix
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            WorldMatrix = Utils.CreateWorldMatrix(Position, Matrix.Identity, new Vector3(Scale, Scale, Scale));
        }


        /// <summary>
        /// The render method. Renders the 
        /// vertices with the help of a vertex and a index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(GameTime gameTime)
        {
            Camera cam = WorldManager.Instance.GetActor("camera") as Camera;

            GameApplication.Instance.GetGraphics().SamplerStates[0] = new SamplerState()
            {
                Filter = textureFilter,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };

            effect.World = WorldMatrix;
            effect.View = cam.View;
            effect.Projection = cam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();

            GameApplication.Instance.GetGraphics().SetVertexBuffer(VertexBuffer);
            GameApplication.Instance.GetGraphics().Indices = IndexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

    }
}
