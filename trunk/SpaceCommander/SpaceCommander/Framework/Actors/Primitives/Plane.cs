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
    public class Plane : Actor
    {

        #region Private
        // those classes are needed in order
        // to create a working plane
        TextureMappingEffect effect;
        VertexBuffer VertexBuffer;
        IndexBuffer IndexBuffer;
        TextureFilter textureFilter = TextureFilter.Linear;
        String _textureFile;
        #endregion

        public Plane(String ID, String textureFile)
            : base(ID, null)
        {
            _textureFile = textureFile;
            this.Scale = Vector3.One;
        }

        public Plane(String ID, String GameViewID, String textureFile)
            : base(ID, GameViewID)
        {
            _textureFile = textureFile;
            this.Scale = Vector3.One;
        }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        public override void LoadContent()
        {
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
            effect = new TextureMappingEffect(ResourceManager.Instance.GetResource<Effect>("TextureMappingEffect").Clone());
            effect.Texture = ResourceManager.Instance.GetResource<Texture2D>(_textureFile);

        }

        /// <summary>
        /// The render method. Render the 
        /// vertices with the help of a vertex and a index buffer
        /// onto the screen.
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public override void Render(SceneGraphManager sceneGraph)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

            GameApplication.Instance.GetGraphics().SamplerStates[0] = new SamplerState()
            {
                Filter = textureFilter,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap
            };

            effect.World = AbsoluteTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;

            effect.CurrentTechnique.Passes[0].Apply();

            GameApplication.Instance.GetGraphics().SetVertexBuffer(VertexBuffer);
            GameApplication.Instance.GetGraphics().Indices = IndexBuffer;
            GameApplication.Instance.GetGraphics().DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

    }
}
