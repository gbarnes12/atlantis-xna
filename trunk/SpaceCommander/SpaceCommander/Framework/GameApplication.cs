namespace GameApplicationTools
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
    using Actors;
    using Interfaces;
    using Events;
    using Misc;

    public class GameApplication
    {
        #region Private
        private static GameApplication instance;
        private GraphicsDevice GraphicsDevice { get; set; }
        private Game Game { get; set; }

        // these objects are only on a temporary
        // manner since we will be updating and 
        // drawing the actors in a different place
        // and will therefore be copied to another 
        // class e.g. ApplicationLayer or something. 
        private List<Actor> updateQueue;
        private List<Actor> updateUIQueue;
        private List<Actor> drawQueue;
        private List<Actor> drawUIQueue;
        #endregion

        #region Public
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

        // path variables so we have some better handling
        public String AssetPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String TexturePath { get; set; }

        public String EffectPath { get; set; }

        public String AudioPath { get; set; }

        public String FontPath { get; set; }

        public String ModelPath { get; set; }

        public String UIPath { get; set; }

        public String EditorPath { get; set; }

        public float NearPlane { get; set; }

        public float FarPlane { get; set; }
        #endregion

        private GameApplication()
        {
            updateQueue = new List<Actor>();
            drawQueue = new List<Actor>();
            updateUIQueue = new List<Actor>();
            drawUIQueue = new List<Actor>();
            
            // set our near plane and far plane variables
            NearPlane = .01f;
            FarPlane = 5000;

            // set up all path variables we need thus 
            // we don't create any errors
            AssetPath = "Assets\\";
            TexturePath = AssetPath + "Textures\\";
            AudioPath = AssetPath + "Audio\\";
            EffectPath = AssetPath + "Effects\\";
            FontPath = AssetPath + "Fonts\\";
            UIPath = AssetPath + "UI\\";
            ModelPath = AssetPath + "Models\\";
            EditorPath = "C:\\Users\\Gavin\\Documents\\Programmierung\\Desktop\\C#\\Atlantis\\SpaceCommander\\SpaceCommander\\SpaceCommander\\SpaceCommander\\bin\\x86\\Debug\\Content\\";
            Logger.Instance.LogPath = "Log.txt";

            #if DEBUG
            Logger.Instance.LogType = LogType.Debug;
            #endif
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

        public void ExitGame()
        {
            if (Game != null)
                Game.Exit();
        }
    }
}
