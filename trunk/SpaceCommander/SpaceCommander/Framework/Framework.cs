namespace Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;

    public class Framework
    {
        #region Private
        private static Framework instance; 
        private GraphicsDevice GraphicsDevice { get; set; }
        private Game Game { get; set; }
        #endregion

        public static Framework Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Framework();
                }
                return instance;
            }
        }

        private Framework()
        {

        }


        public GraphicsDevice GetGraphics()
        {
            if (GraphicsDevice != null)
                return GraphicsDevice;
            else
                throw new Exception("GraphicsDevice hasn't been passed over!");
        }

        public Game GetGame()
        {
            if (Game != null)
                return Game;
            else
                throw new Exception("Game hasn't been passed over!");
        }

    }
}
