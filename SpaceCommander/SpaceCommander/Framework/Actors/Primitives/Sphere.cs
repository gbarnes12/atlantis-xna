namespace GameApplicationTools.Actors.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Actors.Cameras;
    using Interfaces;
    using Resources;

    /// <summary>
    /// The class sphere is very useful to draw some debug 
    /// information to the screen by adding this to a actor node
    /// with the same scale information as the bounding sphere of
    /// the parent actor.
    /// 
    /// Author: Dominik Finkbeiner
    /// Version: 1.0
    /// </summary>
    public class Sphere : Actor
    {
        #region Private
        DefaultEffect effect;
        VertexBuffer vertexBuffer;
        VertexBuffer vertexBuffer2;
        VertexBuffer vertexBuffer3;
        IndexBuffer indexBuffer;
        VertexPositionColor[] vertices;
        VertexPositionColor[] vertices2;
        VertexPositionColor[] vertices3;
        #endregion

        public Sphere(String ID, float Scale)
            : base(ID, null)
        {
            this.Position = Position;
            this.Scale = new Vector3(Scale, Scale, Scale);
        }

        public Sphere(String ID, String GameViewID, float Scale)
            : base(ID, GameViewID)
        {
            this.Scale = new Vector3(Scale, Scale, Scale);
        }

        /// <summary>
        /// Get the BoundingSphere with the value of Vector3.Zero and 
        /// a radius of Scale.
        /// </summary>
        /// <returns>Return a valid bounding sphere <see cref="BoundSphere" /></returns>
        public override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.Zero, Scale.Length());
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
            vertices = new VertexPositionColor[18];
            vertices2 = new VertexPositionColor[18];
            vertices3 = new VertexPositionColor[18];
 
            //first 18 vertices for first ring
            for(int x=0;x<360;x+=20)
                vertices[x / 20] = new VertexPositionColor(Vector3.Transform(new Vector3(0f, 1f, 0f), Matrix.Identity*Matrix.CreateRotationX(MathHelper.ToRadians(x))) + Position, Color.Blue);

            //second 18 vertices for second ring
            for (int x = 0; x < 360; x += 20)
                vertices2[x / 20] = new VertexPositionColor(Vector3.Transform(new Vector3(0f, 1f, 0f), Matrix.Identity * Matrix.CreateRotationZ(MathHelper.ToRadians(x))) + Position, Color.Red);

            //second 18 vertices for second ring
            for (int x = 0; x < 360; x += 20)
                vertices3[x / 20] = new VertexPositionColor(Vector3.Transform(new Vector3(1f, 0, 0f), Matrix.Identity * Matrix.CreateRotationY(MathHelper.ToRadians(x))) + Position, Color.Green);


            //create vertexbuffer
            vertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor),vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            vertexBuffer2 = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor), vertices2.Length, BufferUsage.WriteOnly);
            vertexBuffer2.SetData<VertexPositionColor>(vertices2);

            vertexBuffer3 = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor), vertices3.Length, BufferUsage.WriteOnly);
            vertexBuffer3.SetData<VertexPositionColor>(vertices3);

            //create indexbuffer
            uint[] indices = new uint[] { 0, 1, 1,2,2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10,11,11,12,12,13,13,14,14,15,15,16,16,17,17,0};
            indexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
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

            effect.World = Matrix.CreateScale(Scale) * AbsoluteTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.CurrentTechnique.Passes[0].Apply();


            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer);
            GameApplication.Instance.GetGraphics().Indices = indexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices.Length, 0, 18);

            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer2);
            GameApplication.Instance.GetGraphics().Indices = indexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices2.Length, 0, 18);

            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer3);
            GameApplication.Instance.GetGraphics().Indices = indexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices3.Length, 0, 18);
        }
    }
}
