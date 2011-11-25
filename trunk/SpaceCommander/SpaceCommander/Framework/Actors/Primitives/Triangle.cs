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
    public class Triangle : Actor
    {

        #region Private
        // those classes are needed in order
        // to create a working triangle
        DefaultEffect effect;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        VertexPositionColor[] vertices;
        #endregion

        /// <summary>
        /// Just pass over the ID of this
        /// triangle and the position you want
        /// to have it set to!
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Position"></param>
        public Triangle(String ID)
            : base(ID, null)
        {
        }

        /// <summary>
        /// Just pass over the ID of this
        /// triangle and the position you want
        /// to have it set to!
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="GameViewID"></param>
        /// <param name="Position"></param>
        public Triangle(String ID, String GameViewID)
            : base(ID, GameViewID) 
        {
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        public override void LoadContent()
        {
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

            base.LoadContent();
        }

        /// <summary>
        /// The render method. Render the 
        /// vertices with the help of a vertex and index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

            effect.World = AbsoluteTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            effect.CurrentTechnique.Passes[0].Apply();

            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer);
            GameApplication.Instance.GetGraphics().Indices = indexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 3, 0, 1);

            base.Render(sceneGraph);
        }

    }
}
