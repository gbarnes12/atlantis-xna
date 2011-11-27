namespace GameApplicationTools.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Interfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// 
    /// </summary>
    public class UIActor : EventListener, IUIActorNode
    {
        #region Public
        public virtual Vector2 Position { get; set; }

        public virtual float Angle { get; set; }

        public virtual bool Visible { get; set; }

        public virtual float Scale { get; set; }

        public virtual bool Updateable { get; set; }

        public virtual IController Controller { get; set; }

        public virtual String ID { get; set; }

        public virtual SpriteBatch SpriteBatch { get; set; }

        public virtual Rectangle Rectangle { get; set; }

        public virtual Color Color { get; set; }
        #endregion

        public UIActor(String ID)
        {
            this.ID = ID;
            this.Angle = 0f;
            this.Position = Vector2.Zero;
            this.Scale = 1f;
            this.Updateable = true;
            this.Visible = true;
            this.Color = Color.Black;
        }

        public virtual void LoadContent() {}

        public virtual void Update(GameTime gameTime) { }

        public virtual void Render(GameTime gameTime) { }
    }
}
