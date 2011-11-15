namespace Framework.Actors.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    using Interfaces;
    using Resources;
    using Cameras;

    /// <summary>
    /// Some basic box which loads a crate texture
    /// and has the length of 1. This is just used
    /// for some tech demo and testing purposes. 
    /// If you want to change anything you will need
    /// to create your own Box class on basis of this code. 
    /// 
    /// This class inherits from the Actor class and IDrawableActor
    /// interface.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Box : Actor, IDrawableActor
    {
        #region Private
        // those classes are needed in order
        // to create a working box
        VertexBuffer VertextBuffer;
        IndexBuffer IndexBuffer;
        TextureFilter textureFilter = TextureFilter.Linear;
        TextureMappingEffect effect;
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
        #endregion

        public Box(String ID, Vector3 Position, float Scale) 
            : base(ID)
        {
            this.Position = Position;
            this.IsVisible = true;
            this.Scale = Scale;
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topCenter = new Vector2(0.5f, 0.0f);
            Vector2 topRight = new Vector2(1.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 bottomRight = new Vector2(1.0f, 1.0f);

            VertexPositionTexture[] boxData = new VertexPositionTexture[]
            {
                // Front Surface
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f),topLeft), 
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, 1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f),topRight),  

                // Front Surface
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f),topLeft), 
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f),topRight), 

                // Left Surface
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f),topLeft),
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f),topRight),

                // Right Surface
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, 1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f),topLeft),
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f),topRight),

                // Top Surface
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f),topLeft),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f),topRight),

                // Bottom Surface
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f),bottomLeft),
                new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 1.0f),topLeft),
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f),bottomRight),
                new VertexPositionTexture(new Vector3(1.0f, -1.0f, 1.0f),topRight),
            };

            short[] boxIndices = new short[] { 
                0, 1, 2, 2, 1, 3,   
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 10, 9, 11, 
                12, 13, 14, 14, 13, 15, 
                16, 17, 18, 18, 17, 19,
                20, 21, 22, 22, 21, 23
            };

            VertextBuffer = new VertexBuffer(Game1.graphics.GraphicsDevice, VertexPositionTexture.VertexDeclaration, 24, BufferUsage.WriteOnly);
            IndexBuffer = new IndexBuffer(Game1.graphics.GraphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            VertextBuffer.SetData<VertexPositionTexture>(boxData);
            IndexBuffer.SetData<short>(boxIndices);
            boxData = null;
            boxIndices = null;

            effect = new TextureMappingEffect(content.Load<Effect>("Effects\\TextureMappingEffect"));
            effect.Texture = content.Load<Texture2D>("Textures\\crate");

            Game1.graphics.GraphicsDevice.SamplerStates[0] = new SamplerState()
            {
                Filter = textureFilter
            };
        }

        /// <summary>
        /// The Update method. This will
        /// take care of updating our world matrix
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
           WorldMatrix = Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(Scale, Scale, Scale));
        }

        /// <summary>
        /// The render method. Renders the 
        /// vertices with the help of a vertex and index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            FPSCamera cam = WorldManager.Instance.GetActor<FPSCamera>("camera");

            effect.World = WorldMatrix;
            effect.View = cam.View;
            effect.Projection = cam.Projection;

            effect.CurrentTechnique.Passes[0].Apply();

            Game1.graphics.GraphicsDevice.SetVertexBuffer(VertextBuffer);
            Game1.graphics.GraphicsDevice.Indices = IndexBuffer;
            Game1.graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);
        }
    }
}
