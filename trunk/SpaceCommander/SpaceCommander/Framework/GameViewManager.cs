namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Structures;
    using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

    public class GameViewManager
    {
        #region Private
        private Dictionary<String, GameView> _gameViews;
        private static GameViewManager instance;
        #endregion

        #region Public
        /// <summary>
        /// Retrieves the current and only instance
        /// of the GameViewManager Object within our 
        /// project.
        /// 
        /// This is represents the singleton pattern.
        /// </summary>
        public static GameViewManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameViewManager();
                }
                return instance;
            }
        }
        #endregion

        private GameViewManager()
        {
            this._gameViews = new Dictionary<string, GameView>();
        }

        /// <summary>
        /// Retrieves a game view within our 
        /// game view list and casts him to a
        /// given game view class. 
        /// 
        /// You could use GetGameView(ID) instead.
        /// </summary>
        /// <typeparam name="T">The game view type to be casted to</typeparam>
        /// <param name="ID">The id of the game view you want to retrieve</param>
        /// <returns>Will return a object of the Type T</returns>
        public T GetGameView<T>(String ID)
        {
            try
            {
                // we need to pass over null as third argument 
                // because due to the fact that XBOX 360 needs this 
                // third parameter. 
                return (T)Convert.ChangeType(_gameViews[ID], typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }


        /// <summary>
        /// Retrieves a game view within our 
        /// game view list and casts him to a
        /// given game view class. 
        /// </summary>
        /// <param name="ID">The id of the game view you want to retrieve</param>
        /// <returns>Instance of the IGameView interface within our dictionary.</returns>
        public GameView GetGameView(String ID)
        {
            if (_gameViews.ContainsKey(ID))
                return _gameViews[ID];
            else
                return null;
        }

        /// <summary>
        /// Returns the whole dictionary of game views 
        /// to some circumstances this can be very
        /// useful and will come in handy. 
        /// </summary>
        /// <returns>The Dictionary<String, Actor></returns>
        public Dictionary<String, GameView> GetGameViews()
        {
            return this._gameViews;
        }

        /// <summary>
        /// Adds any actor class to the dictionary.
        /// You have to assign an id on your own 
        /// otherwise it will fail.
        /// </summary>
        /// <param name="actor">The instance of the actor class you want to pass over</param>
        public void AddGameView(GameView gameView)
        {
            if (_gameViews != null)
            {
                if (gameView.ID != null || gameView.ID != "" || gameView.ID != " ")
                {
                    if (!_gameViews.ContainsKey(gameView.ID))
                        this._gameViews.Add(gameView.ID, gameView);
                    else
                        throw new Exception("There is already an actor with the id: " + gameView.ID);
                }
                else
                    throw new Exception("Your actor has no valid ID or even no ID assigned. Please assign some id to it!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (_gameViews.Count > 0)
            {
                foreach (GameView gameView in _gameViews.Values)
                {
                    if (!gameView.BlocksUpdating)
                    {
                        gameView.Update(gameTime);
                    }
                }
            }
        }

        public void Render()
        {
            if (_gameViews.Count > 0)
            {
                foreach (GameView gameView in _gameViews.Values)
                {
                    if (!gameView.BlocksRendering)
                    {
                        gameView.Render();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent()
        {
            if (_gameViews.Count > 0)
            {
                foreach (GameView gameView in _gameViews.Values)
                {
                    if (!gameView.BlocksLoading)
                    {
                        gameView.LoadContent();
                    }
                }
            }
        }

    }
}
