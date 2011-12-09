using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Actors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameApplicationTools.UI
{
    public class TextureElement : UIActor
    {
        protected Texture2D texture;

        public TextureElement(String ID, Vector2 position,Texture2D texture) 
            :base(ID)
        {
            this.Position = position;
            this.texture = texture;

            // initialize our sprite batch
            if (GameApplication.Instance.GetGraphics() != null)
                SpriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());
        }

        /// <summary>
        /// Will Render the TextElement: Basically just says the sprite batch
        /// to begin its duty and draw a string to the screen with our 
        /// information provided.
        /// </summary>
        /// <param name="gameTime">The GameTime which we pass over to this element.</param>
        public override void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(texture, Position,null, Color, Angle, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            SpriteBatch.End();

            GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default;
            GameApplication.Instance.GetGraphics().SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
