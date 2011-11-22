namespace GameApplicationTools.Actors.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using UI;
    using Interfaces;
    using Cameras;
    using Events;
    using Resources;
    using Misc;

    /// <summary>
    /// This will create a normal triangle with the length of 1 which
    /// in our world represents 1 meter. Its mainly used for debugging 
    /// purposes or tech demos and just demonstrates the usage of 
    /// vertices and a shader. 
    /// 
    /// This class inherits from the actor class and the IDrawableActor 
    /// interface. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Triangle : Actor, IDrawableActor
    {

        #region Private
        // those classes are needed in order
        // to create a working triangle
        DefaultEffect effect;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        SpriteBatch spriteBatch;
        VertexPositionColor[] vertices;
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

        public Matrix RotationMatrix { get; set; }

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


        /// <summary>
        /// Just pass over the ID of this
        /// triangle and the position you want
        /// to have it set to!
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Position"></param>
        public Triangle(String ID, Vector3 Position)
            : base(ID, null)
        {
            this.Position = Position;
            this.Angle = 0f;
            this.IsVisible = true;
            this.IsUpdateable = true;

            WorldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Just pass over the ID of this
        /// triangle and the position you want
        /// to have it set to!
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="GameViewID"></param>
        /// <param name="Position"></param>
        public Triangle(String ID, String GameViewID, Vector3 Position)
            : base(ID, GameViewID) 
        {
            this.Position = Position;
            this.Angle = 0f;
            this.IsVisible = true;
            this.IsUpdateable = true;

            WorldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent()
        {
            if (GameApplication.Instance.GetGraphics() != null)
                spriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());

            // load some basiseffect
            effect = new DefaultEffect(ResourceManager.Instance.GetResource<Effect>("DefaultEffect").Clone());


            // set up our vertices
            vertices = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(0f, 1.0f, 0f), Color.Green),
                new VertexPositionColor(new Vector3(1.0f, 0f, 0f), Color.Red),
                new VertexPositionColor(new Vector3(-1.0f, 0f, 0f), Color.Blue),
            };

            //create vertexbuffer
            vertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            //create indexbuffer
            uint[] indices = new uint[] { 0, 1, 2 };
            indexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.ThirtyTwoBits, 3, BufferUsage.WriteOnly);
            indexBuffer.SetData<uint>(indices);
        }


        /// <summary>
        /// The Update method. This will
        /// take care of updating our world matrix
        /// and let the triangle rotate. If you
        /// want to create a none rotating triangle
        /// just create a new class and copy this one.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Angle += 0.005f;
            WorldMatrix = Utils.CreateWorldMatrix(Vector3.Zero, Matrix.CreateRotationX(Angle));
        }

        /// <summary>
        /// The render method. Render's the 
        /// vertices with the help of a vertex and index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(GameTime gameTime)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

            effect.World = WorldMatrix;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            effect.CurrentTechnique.Passes[0].Apply();

            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer);
            GameApplication.Instance.GetGraphics().Indices = indexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 3, 0, 1);
        }

    }
}
