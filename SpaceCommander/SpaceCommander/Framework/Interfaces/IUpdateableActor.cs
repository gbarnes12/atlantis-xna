namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;


    /// <summary>
    /// You can create an actor that inherits from
    /// this interface which will tell the user that
    /// this is only an actor that only gets updated
    /// and not drawn.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IUpdateableActor
    {
        /// <summary>
        /// The Position of this Actor 
        /// in the World. 
        /// </summary>
        Vector3 Position { get; set; }

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
        void LoadContent();

        /// <summary>
        /// The Update method. Just write your 
        /// update stuff for the specific actor in
        /// here. 
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
    }
}
