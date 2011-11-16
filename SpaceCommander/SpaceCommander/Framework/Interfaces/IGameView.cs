namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This interface needs to be implemented into
    /// every game view we have in the game!
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IGameView
    {
        #region Public
        /// <summary>
        /// Determines whether rendering should be processed 
        /// or not for specific actors generated within this game view.
        /// </summary>
        bool BlocksRendering { set; get; }

        /// <summary>
        /// Determines whether updating should be processed 
        /// or not for specific actors.
        /// </summary>
        bool BlocksUpdating { get; set; }

        /// <summary>
        /// Can be used to block any input within the current game view
        /// </summary>
        bool BlocksInput { get; set; }

        /// <summary>
        /// Can be used to block any loading processes.
        /// </summary>
        bool BlocksLoading { get; set; }
        #endregion

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

    }
}
