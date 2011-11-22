namespace GameApplicationTools.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public struct Resource
    {
        /// <summary>
        /// 
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ResourceType Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ResourceType
    {
        Texture2D,
        Texture3D,
        Effect,
        Model,
        SoundEffect,
        Song,
        Video,
        SpriteFont
    }
}
