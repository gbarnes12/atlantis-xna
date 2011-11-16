namespace GameApplicationTools.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Actors;
    using GameApplicationTools.Interfaces;

    /// <summary>
    /// A GameView is used to represent the game's 
    /// displays it can be everything from HUD to game play. 
    /// Its used to separate things e.g. menus from game play screens
    /// and so on. 
    /// 
    /// You need to implement the IGameView interface otherwise
    /// it won't work properly. This is just a basic body of the
    /// class.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class GameView
    {
        #region Public
        /// <summary>
        /// This is used to have an unique identifier
        /// for the specific GameView within the game.
        /// </summary>
        public String ID { get; set; }
        #endregion

        public GameView(string ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Sets every actor which has this
        /// game view id to the current state!
        /// </summary>
        /// <param name="value"></param>
        public void SetActorsVisibility(bool value)
        {
            Dictionary<String, Actor> actors = WorldManager.Instance.GetActors();
            foreach (Actor actor in actors.Values)
            {
                if(actor is IDrawableActor)
                {
                    IDrawableActor actorDrawable = (IDrawableActor)actor;
                    if (actor.GameViewID == ID)
                    {
                        actorDrawable.IsVisible = value;
                    }
                }
                else if (actor is IUIActor)
                {
                    IUIActor actorDrawable = (IUIActor)actor;
                    if (actor.GameViewID == ID)
                    {
                        actorDrawable.IsVisible = value;
                    }
                }
            }
        }

        /// <summary>
        /// Sets every actor which has this
        /// game view id to the current state!
        /// </summary>
        /// <param name="value"></param>
        public void SetActorsUpdateable(bool value)
        {
            Dictionary<String, Actor> actors = WorldManager.Instance.GetActors();
            foreach (Actor actor in actors.Values)
            {
                if (actor is IDrawableActor)
                {
                    IDrawableActor actorUpdatable = (IDrawableActor)actor;
                    if (actor.GameViewID == ID)
                    {
                        actorUpdatable.IsUpdateable = value;
                    }
                }
                else if (actor is IUIActor)
                {
                    IUIActor actorUpdatable = (IUIActor)actor;
                    if (actor.GameViewID == ID)
                    {
                        actorUpdatable.IsUpdateable = value;
                    }
                }
                else if (actor is IUpdateableActor)
                {
                    IUpdateableActor actorUpdatable = (IUpdateableActor)actor;
                    if (actor.GameViewID == ID)
                    {
                        actorUpdatable.IsUpdateable = value;
                    }
                }
            }
        }
    }
}
