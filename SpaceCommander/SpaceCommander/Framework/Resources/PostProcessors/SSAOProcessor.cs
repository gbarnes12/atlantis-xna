namespace GameApplicationTools.Resources.PostProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Actors.Cameras;

    public class SSAOProcessor : PostProcessor
    {
        #region Private
        Texture2D randomNormalsTexture;
        #endregion

        #region Public
        //Sample Radius
        public float SampleRadius { get; set; }
        //Distance Scale
        public float DistanceScale { get; set; }
        #endregion

        public SSAOProcessor(string ID, string FileName, string RandomNormals)
            : base(ID, FileName)
        {
            SampleRadius = 0;
            DistanceScale = 0;
            randomNormalsTexture = ResourceManager.Instance.GetResource<Texture2D>(RandomNormals);
        }

        public override void Update(Microsoft.Xna.Framework.Graphics.Texture2D result)
        {
            //GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            //GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default;
            //GameApplication.Instance.GetGraphics().RasterizerState = RasterizerState.CullCounterClockwise;

            Camera cam = CameraManager.Instance.GetCurrentCamera();

            Vector3 cornerFrustum = Vector3.Zero;
            cornerFrustum.Y = (float)Math.Tan(Math.PI / 3.0 / 2.0) * cam.FarPlane;
            cornerFrustum.X = cornerFrustum.Y * cam.AspectRatio;
            cornerFrustum.Z = cam.FarPlane;

            //Set SSAO parameters
            Effect.Parameters["Projection"].SetValue(cam.Projection);
            Effect.Parameters["cornerFustrum"].SetValue(cornerFrustum);
            Effect.Parameters["sampleRadius"].SetValue(SampleRadius);
            Effect.Parameters["distanceScale"].SetValue(DistanceScale);
            Effect.Parameters["GBufferTextureSize"].SetValue(new Vector2(result.Width, result.Height));

            base.Update(result);
        }

        public override Texture2D Render(Texture2D result, SpriteBatch sceneDrawer)
        {
            return base.Render(result, sceneDrawer);
        }
    }
}
