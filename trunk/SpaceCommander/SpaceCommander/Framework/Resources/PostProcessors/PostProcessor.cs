﻿namespace GameApplicationTools.Resources.PostProcessors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    public class PostProcessor
    {
        #region Public
        public string ID { get; set; }
        public Effect Effect { get; set; }
        public bool Enabled { get; set; }
        public RenderTarget2D Target { get; set; }
        #endregion

        public PostProcessor(string ID, string effectName)
        {
            this.ID = ID;
            this.Effect = ResourceManager.Instance.GetResource<Effect>(effectName);
            this.Enabled = true;

            PresentationParameters pp = GameApplication.Instance.GetGraphics().PresentationParameters;
            this.Target = new RenderTarget2D(GameApplication.Instance.GetGraphics(), pp.BackBufferWidth, pp.BackBufferHeight, false,
                GameApplication.Instance.GetGraphics().DisplayMode.Format, DepthFormat.Depth24Stencil8);
        }

        public virtual void Update(Texture2D result) {}

        public virtual Texture2D Render(Texture2D result, SpriteBatch sceneDrawer)
        {
            sceneDrawer.Begin(0, BlendState.Opaque, null, null, null, Effect);
            sceneDrawer.Draw(result, Vector2.Zero, Color.White);
            sceneDrawer.End();

            return result;
        }
    }
}