namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;


    /// <summary>
    /// Use this to determine that you want
    /// to declare your actor as an UI component.
    /// You will have to implement all the properties
    /// and method thus the actor works right. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IUIActorNode
    {
        #region Public
        /// <summary>
        /// The Position of this Actor 
        /// in the World. 
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Sets the current angle of this 
        /// Actor.
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets drawn or not
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Sets the scale of our model
        /// </summary>
        float Scale { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        bool Updateable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IController Controller { get; set; }

        /// <summary>
        /// The sprite batch which can be used
        /// to draw some stuff
        /// </summary>
        SpriteBatch SpriteBatch { get; set; }

        /// <summary>
        /// A rectangle used to check collision with the mouse 
        /// or something else. 
        /// </summary>
        Rectangle Rectangle { get; set; }

        Color Color { get; set; }
        #endregion

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// The Update method. Just write your 
        /// update stuff for the specific actor in
        /// here. 
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);

        /// <summary>
        /// The render method. Just write your 
        /// render stuff for the specific actor in
        /// here. 
        /// </summary>
        /// <param name="gameTime"></param>
        void Render(GameTime gameTime);

       
    }
}
