namespace GameApplicationTools.Actors.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;

    using Cameras;
    using Events;
    using UI;
    using Interfaces;
    using Resources;
    using Misc;

    /// <summary>
    /// Will draw a basic axis to the screen with the length of 1. 
    /// The different Axis are colored thus you have blue for Y, 
    /// red for X and green for Z. 
    /// 
    /// This class inherits from the Actor class and IDrawableActor
    /// interface. 
    /// 
    /// Its main usage is for any debugging purposes or tech demos. 
    /// So you can look where your origin is, but since you can assign
    /// a position it may come in handy for some sort of map editor or so.
    /// 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Axis : Actor, IDrawableActor
    {
        #region Private
        // those classes are needed in order
        // to create a working axis
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

        public Axis(String ID, Vector3 Position, float Scale) 
            : base(ID, null) 
        {
            this.Position = Position;
            this.Angle = 0f;
            this.Scale = Scale;
            this.IsVisible = true;
            this.IsUpdateable = true;

            WorldMatrix = Matrix.Identity;
        }


        public Axis(String ID, String GameViewID, Vector3 Position, float Scale)
            : base(ID, GameViewID)
        {
            this.Position = Position;
            this.Angle = 0f;
            this.Scale = Scale;
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
           
            // load some basiseffect
            effect = new DefaultEffect(ResourceManager.Instance.GetResource<Effect>("DefaultEffect").Clone());
 

            // set up our vertices
            vertices = new VertexPositionColor[] {
                new VertexPositionColor(new Vector3(0f, 0.0f, 0f), Color.Blue),
                new VertexPositionColor(new Vector3(0f, 1f, 0f), Color.Blue),

                new VertexPositionColor(new Vector3(0f, 0f, 0f), Color.Red),
                new VertexPositionColor(new Vector3(1f, 0f, 0f), Color.Red),

                new VertexPositionColor(new Vector3(0f, 0f, 0f), Color.Green),
                new VertexPositionColor(new Vector3(0f, 0f, 1f), Color.Green),
            };

            //create vertexbuffer
            vertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            //create indexbuffer
            try
            {
                uint[] indices = new uint[] { 0, 1, 2, 3, 4, 5 };
                indexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.ThirtyTwoBits, 6, BufferUsage.WriteOnly);
                indexBuffer.SetData<uint>(indices);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// The Update method. This will
        /// take care of updating our world matrix
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            WorldMatrix = Utils.CreateWorldMatrix(Position, Matrix.CreateRotationY(Angle), new Vector3(Scale, Scale, Scale));
        }

        /// <summary>
        /// The render method. Renders the 
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
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 6, 0, 3);

        }
    }
}
