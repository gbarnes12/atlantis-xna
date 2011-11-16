namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    using Actors;
    using Interfaces;
    using Events;

    /// <summary>
    /// Represents a global access point for every
    /// actor in our world. This is used as a singleton
    /// thus there can only be one instance per time since
    /// we are only in the need of one instance this is perfectly
    /// well suited to our needs.
    /// 
    /// Example:
    /// WorldManager.Instance.GetActor(string ID);
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class WorldManager
    {
        #region Private
        private Dictionary<String, Actor> _actors;
        private static WorldManager instance;
        #endregion

        /// <summary>
        /// Retrieves the current and only instance
        /// of the WorldManager Object within our 
        /// project.
        /// 
        /// This is represents the singleton pattern.
        /// </summary>
        public static WorldManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorldManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Sets up all the necessary objects and 
        /// properties and will create new instances
        /// of those.
        /// </summary>
        private WorldManager()
        {
            _actors = new Dictionary<String, Actor>();
        }

        /// <summary>
        /// Retrieves an actor within our 
        /// actor list and casts him to a
        /// given actor class. 
        /// 
        /// You could use GetActor(ID) instead.
        /// </summary>
        /// <typeparam name="T">The actor type to be casted to</typeparam>
        /// <param name="ID">The id of the actor you want to retrieve</param>
        /// <returns>Will return a object of the Type T</returns>
        public T GetActor<T>(String ID) 
        {
            try
            {
                // we need to pass over null as third argument 
                // because due to the fact that XBOX 360 needs this 
                // third parameter. 
                return (T)Convert.ChangeType(_actors[ID], typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Returns an actor of the list 
        /// just with the Base Actor class. 
        /// This won't handle casting issues for 
        /// you. 
        /// </summary>
        /// <param name="ID">The id of the actor you want to retrieve</param>
        /// <returns>Instance of the actor class within our dictionary.</returns>
        public Actor GetActor(String ID)
        {
            if (_actors.ContainsKey(ID))
                return _actors[ID];
            else
                return null;
        }

        /// <summary>
        /// Returns the whole dictionary of actors 
        /// to some circumstances this can be very
        /// useful and will come in handy. 
        /// </summary>
        /// <returns>The Dictionary<String, Actor></returns>
        public Dictionary<String, Actor> GetActors()
        {
            return this._actors;
        }

        /// <summary>
        /// Adds any actor class to the dictionary.
        /// You have to assign an id on your own 
        /// otherwise it will fail.
        /// </summary>
        /// <param name="actor">The instance of the actor class you want to pass over</param>
        public void AddActor(Actor actor)
        {
            if (_actors != null)
            {
                if (actor.ID != null || actor.ID != "" || actor.ID != " ")
                {
                    if (!_actors.ContainsKey(actor.ID))
                        this._actors.Add(actor.ID, actor);
                    else
                        throw new Exception("There is already an actor with the id: " + actor.ID);
                }
                else
                    throw new Exception("Your actor has no valid ID or even no ID assigned. Please assign some id to it!");
            }
        }
    }
}
