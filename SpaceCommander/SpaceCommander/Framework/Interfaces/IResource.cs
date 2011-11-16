namespace GameApplication.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Represents a basic resource such as a texture
    /// or effect
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// The body of a load content method which
        /// allows us to load some basic stuff in here.
        /// </summary>
        /// <param name="content"></param>
        void LoadContent(ContentManager content);
    }
}
