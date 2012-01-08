namespace GameApplicationTools.Actors.Primitives
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
    using Misc;
    using GameApplicationTools.Actors.Properties;

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
    /// Version: 1.2
    /// </summary>
    public class Box : Actor
    {
        #region Private
        // those classes are needed in order
        // to create a working box
        VertexBuffer VertextBuffer;
        IndexBuffer IndexBuffer;
        TextureFilter textureFilter = TextureFilter.Linear;
        Effect effect;
        Sphere sphere;
        String _textureName;
        String _normalMapTexture;
        #endregion

        public Box(String ID, String textureName, String normalMapTexture ,float Scale) 
            : base(ID, null)
        {
            _textureName = textureName;
            _normalMapTexture = normalMapTexture;
            this.Scale = new Vector3(Scale, Scale, Scale);
            sphere = new Sphere(ID + "_sphere", Scale);
            this.Children.Add(sphere);

            // create properties
            PickableProperty pickableProperty = new PickableProperty();
            Properties.Add(ActorPropertyType.PICKABLE, pickableProperty);
        }

        public Box(String ID, String textureName, String normalMapTexture, String GameViewID, float Scale)
            : base(ID, GameViewID)
        {
            _textureName = textureName;
            _normalMapTexture = normalMapTexture;
            this.Scale = new Vector3(Scale, Scale, Scale);
            sphere = new Sphere(ID + "_sphere", Scale);
            this.Children.Add(sphere);

            // create properties
            PickableProperty pickableProperty = new PickableProperty();
            Properties.Add(ActorPropertyType.PICKABLE, pickableProperty);
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        public override void LoadContent()
        {
            sphere.LoadContent();

            Vector2 topLeft = new Vector2(0.0f, 0.0f);
            Vector2 topCenter = new Vector2(0.5f, 0.0f);
            Vector2 topRight = new Vector2(1.0f, 0.0f);
            Vector2 bottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 bottomRight = new Vector2(1.0f, 1.0f);

            VertexPositionNormalTexture[] boxData = new VertexPositionNormalTexture[]
            {
                // Front Surface
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), Vector3.Up ,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), Vector3.Up ,topLeft), 
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f), Vector3.Up ,bottomRight),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), Vector3.Up ,topRight),  

                // Front Surface
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), Vector3.Up,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), Vector3.Up,topLeft), 
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), Vector3.Up,bottomRight),
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), Vector3.Up,topRight), 

                // Left Surface
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), Vector3.Up,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), Vector3.Up,topLeft),
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), Vector3.Up,bottomRight),
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), Vector3.Up,topRight),

                // Right Surface
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f), Vector3.Up,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), Vector3.Up,topLeft),
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), Vector3.Up,bottomRight),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), Vector3.Up,topRight),

                // Top Surface
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, 1.0f), Vector3.Up,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-1.0f, 1.0f, -1.0f), Vector3.Up,topLeft),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, 1.0f), Vector3.Up,bottomRight),
                new VertexPositionNormalTexture(new Vector3(1.0f, 1.0f, -1.0f), Vector3.Up,topRight),

                // Bottom Surface
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, -1.0f), Vector3.Up,bottomLeft),
                new VertexPositionNormalTexture(new Vector3(-1.0f, -1.0f, 1.0f), Vector3.Up,topLeft),
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, -1.0f), Vector3.Up,bottomRight),
                new VertexPositionNormalTexture(new Vector3(1.0f, -1.0f, 1.0f), Vector3.Up,topRight),
            };

            short[] boxIndices = new short[] { 
                0, 1, 2, 2, 1, 3,   
                4, 5, 6, 6, 5, 7,
                8, 9, 10, 10, 9, 11, 
                12, 13, 14, 14, 13, 15, 
                16, 17, 18, 18, 17, 19,
                20, 21, 22, 22, 21, 23
            };

            VertextBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), VertexPositionNormalTexture.VertexDeclaration, 24, BufferUsage.WriteOnly);
            IndexBuffer = new IndexBuffer(GameApplication.Instance.GetGraphics(), IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            VertextBuffer.SetData<VertexPositionNormalTexture>(boxData);
            IndexBuffer.SetData<short>(boxIndices);
            boxData = null;
            boxIndices = null;

            effect = ResourceManager.Instance.GetResource<Effect>("NormalMappingEffect").Clone();
            effect.Parameters["DiffuseTexture"].SetValue(ResourceManager.Instance.GetResource<Texture2D>(_textureName));
            effect.Parameters["TextureEnabled"].SetValue(true);
            effect.Parameters["NormalTexture"].SetValue(ResourceManager.Instance.GetResource<Texture2D>(_normalMapTexture));
        }

        /// <summary>
        /// Get the BoundingSphere with the value of Vector3.Zero and 
        /// a radius of Scale.
        /// </summary>
        /// <returns>Return a valid bounding sphere <see cref="BoundSphere" /></returns>
        public override BoundingSphere GetBoundingSphere()
        {
            float radius = Scale.Y * 2;
            if(radius == 0)
                radius = 1f;

            return new BoundingSphere(Vector3.Zero, radius);
        }

        /// <summary>
        /// Sets the SampleStates Filter to a specific filter we have chosen!
        /// </summary>
        public override void PreRender()
        {
            GameApplication.Instance.GetGraphics().SamplerStates[0] = new SamplerState()
            {
                Filter = textureFilter
            };

            base.PreRender();
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

            effect.Parameters["World"].SetValue(AbsoluteTransform);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);

            effect.CurrentTechnique.Passes[0].Apply();

            GameApplication.Instance.GetGraphics().SetVertexBuffer(VertextBuffer);
            GameApplication.Instance.GetGraphics().Indices = IndexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12);

            base.Render(sceneGraph);
        }
    }
}
