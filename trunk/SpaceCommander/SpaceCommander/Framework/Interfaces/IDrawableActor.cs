namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// This is the opposite of the IUpdateableActor
    /// and has not only the update method but the render
    /// method, too. Besides it stores all the necessary
    /// information for an actor that needs to get drawn to
    /// the screen. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IDrawableActor
    {
        /// <summary>
        /// The Position of this Actor 
        /// in the World. 
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Sets the current angle of this 
        /// Actor.
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// Sets the scale of our model
        /// </summary>
        float Scale { get; set; }

        /// <summary>
        /// The world matrix of the inheriting actor
        /// that we need to set the current scale, position
        /// and rotation.
        /// </summary>
        Matrix WorldMatrix { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets drawn or not
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Determines whether the 
        /// actor gets updated or not
        /// </summary>
        bool IsUpdateable { get; set; }

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
