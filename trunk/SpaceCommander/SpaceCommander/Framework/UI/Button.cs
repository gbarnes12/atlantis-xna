namespace GameApplicationTools.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Input;

    using Actors;
    using Interfaces;
    using Events;
    using Input;


    public class Button : UIActor
    {
        #region Public
        /// <summary>
        /// button's background-texture
        /// </summary>
        public Texture2D Texture
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// create a new button to click on and fire an event
        /// </summary>
        /// <param name="id">actor's (button) id</param>
        /// <param name="position">position relative to the upper-left corner (0|0)</param>
        /// <param name="texture">background-texture of the button</param>
        /// <param name="width">button's width (has not to be the same as the baackground-texture's width)</param>
        /// <param name="height">button's heighth(as not to be the same as the baackground-texture's width)</param>
        public Button(String ID, Vector2 position, Texture2D texture, int width, int height)
            : base(ID)
        {
            this.Position = position;
            this.Texture = texture;

            this.Rectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            // initialize our sprite batch
            if (GameApplication.Instance.GetGraphics() != null)
                SpriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());
        }



        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (MouseDevice.Instance.WasButtonPressed(MouseButtons.Left))
            {
                if (new Rectangle(MouseDevice.Instance.State.X, MouseDevice.Instance.State.Y, 1, 1).Intersects(Rectangle))
                {
                    //fire event ! onClickButton
                    ButtonEvent_OnClick Event = new ButtonEvent_OnClick();
                    Event.Sender = this;
                    EventManager.Instance.HookEvent(EventType.ButtonEvent_OnClick, Event);
                }
            }
        }

        public override void Render(GameTime gameTime)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(Texture, Rectangle, Color.White);
            SpriteBatch.End();

            GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default;
            GameApplication.Instance.GetGraphics().SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
