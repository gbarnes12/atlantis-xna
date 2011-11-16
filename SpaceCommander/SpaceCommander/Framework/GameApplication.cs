namespace GameApplication
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

    public class GameApplication
    {
        #region Private
        private static GameApplication instance;
        private GraphicsDevice GraphicsDevice { get; set; }
        private Game Game { get; set; }
        #endregion

        public static GameApplication Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameApplication();
                }
                return instance;
            }
        }

        private GameApplication()
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

        public void SetGraphicsDevice(GraphicsDevice graphics)
        {
            this.GraphicsDevice = graphics;
        }

        public void SetGame(Game game)
        {
            this.Game = game;
        }

    }
}
