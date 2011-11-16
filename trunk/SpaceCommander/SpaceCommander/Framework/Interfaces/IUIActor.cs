namespace GameApplication.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;


    /// <summary>
    /// Use this to determine that you want
    /// to declare your actor as an UI component.
    /// You will have to implement all the properties
    /// and method thus the actor works right. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    interface IUIActor
    {
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
        bool IsVisible { get; set; }

        /// <summary>
        /// Sets the scale of our model
        /// </summary>
        float Scale { get; set; }

        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        void LoadContent(ContentManager content);

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
