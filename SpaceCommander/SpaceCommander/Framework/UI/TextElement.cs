namespace GameApplicationTools.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using Actors;
    using Interfaces;

    /// <summary>
    /// This creates an instance of a 
    /// TextElement which is basically just 
    /// some text on the screen. Could be
    /// used to represent any debug information
    /// or some sort of menu points. 
    /// 
    /// This class inherits from Actor and IUIActor.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class TextElement : UIActor
    {
        #region Private
        SpriteFont spriteFont;
        #endregion

        #region Public
        /// <summary>
        /// The current color of our text element. 
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The message or text you want to print to the screen.
        /// </summary>
        public String Text { get; set; }
        #endregion

        /// <summary>
        /// Creates a new TextElement but make sure to not 
        /// forget to assign it to our world manager. 
        /// </summary>
        /// <param name="ID">The ID of this TextElement</param>
        /// <param name="Position">The position on the screen.</param>
        /// <param name="Color">Some sort of color.</param>
        /// <param name="Text">The message you want to present</param>
        /// <param name="Font">And finally the type of font you want to use</param>
        public TextElement(String ID, Vector2 Position, 
            Color Color, String Text, SpriteFont Font) 
            : base(ID)
        {
            this.Position = Position;
            this.Color = Color;
            this.Text = Text;

            this.Angle = 0f;
            this.Scale = 1f;

            // initialize our sprite batch
            if (GameApplication.Instance.GetGraphics() != null)
                SpriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());

            spriteFont = Font;
        }


        /// <summary>
        /// Here you could perform necessary loading of some stuff 
        /// you need to load but currently it doesn't do anything.
        /// </summary>
        /// <param name="content">An instance of the ContentManager we can use</param>
        public override void LoadContent() { }

        /// <summary>
        /// Will update the TextElement, but due to the fact
        /// that this is just a basic TextElement we don't have 
        /// to perform any updating.
        /// </summary>
        /// <param name="gameTime">The GameTime which we pass over to this element.</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime) { }

        /// <summary>
        /// Will Render the TextElement: Basically just says the sprite batch
        /// to begin its duty and draw a string to the screen with our 
        /// information provided.
        /// </summary>
        /// <param name="gameTime">The GameTime which we pass over to this element.</param>
        public override void Render(Microsoft.Xna.Framework.GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.DrawString(spriteFont, Text, Position, Color, Angle, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            SpriteBatch.End();

            GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default;
            GameApplication.Instance.GetGraphics().SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
