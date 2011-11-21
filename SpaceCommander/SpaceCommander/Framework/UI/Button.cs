namespace GameApplicationTools.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Actors;
    using Interfaces;
    using Microsoft.Xna.Framework.Input;
    using Events;
    using GameApplicationTools.Input;


    public class Button : Actor, IUIActor
    {
        #region Public

        /// <summary>
        ///The current position of our 
        /// text relative to the left upper 
        /// corner (0,0).
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// The current angle which takes care 
        /// of rotating the text we want to present.
        /// Default is set to 0f
        /// </summary>
        public float Angle
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether this TextElement 
        /// should get drawn or not!
        /// </summary>
        public bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// button's scale
        /// </summary>
        public float Scale
        {
            get;
            set;
        }

        /// <summary>
        /// button's background-texture
        /// </summary>
        public Texture2D Texture
        {
            get;
            set;
        }

        /// <summary>
        /// button's bounding-box, checks collision with mouse 
        /// </summary>
        public Rectangle Rectangle
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        public bool IsUpdateable { get; set; }
        #endregion

        #region Private

        private SpriteBatch spriteBatch;

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
            : base(ID, null)
        {
            this.Position = position;
            this.IsUpdateable = true;
            this.Texture = texture;

            this.Rectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            // initialize our sprite batch
            if (GameApplication.Instance.GetGraphics() != null)
                spriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());
        }

        /// <summary>
        /// create a new button to click on and fire an event
        /// </summary>
        /// <param name="id">actor's (button) id</param>
        /// <param name="position">position relative to the upper-left corner (0|0)</param>
        /// <param name="texture">background-texture of the button</param>
        /// <param name="width">button's width (has not to be the same as the baackground-texture's width)</param>
        /// <param name="height">button's heighth(as not to be the same as the baackground-texture's width)</param>
        public Button(String ID, String GameViewID ,Vector2 position, Texture2D texture, int width, int height)
            : base(ID, GameViewID)
        {
            this.Position = position;
            this.Texture = texture;

            this.Rectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            // initialize our sprite batch
            if (GameApplication.Instance.GetGraphics() != null)
                spriteBatch = new SpriteBatch(GameApplication.Instance.GetGraphics());
        }

        public void LoadContent(ContentManager content)
        {
        }

        public void Update(GameTime gameTime)
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

        public void Render(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Rectangle, Color.White);
            spriteBatch.End();

            GameApplication.Instance.GetGraphics().BlendState = BlendState.Opaque;
            GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default;
            GameApplication.Instance.GetGraphics().SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
