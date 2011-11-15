namespace Framework
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
        #region Private Properties
        private Dictionary<String, Actor> _actors;
        private static WorldManager instance;

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

            updateQueue = new List<Actor>();
            drawQueue = new List<Actor>();
            updateUIQueue = new List<Actor>();
            drawUIQueue = new List<Actor>();
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

        /// <summary>
        /// Updates all the actors within the 
        /// WorldManager dictionary, this will
        /// update the Actors which inherit from 
        /// IUpdateableActor and IDrawableActor 
        /// at first. 
        /// Afterwards it updates all the IUIActors.
        /// </summary>
        /// <param name="gameTime">The current gameTime object from the base game class</param>
        public void Update(GameTime gameTime)
        {
            // Sort all the actors and add them to a temporary queue
            // depending on their inherited interfaces.
            foreach (Actor actor in GetActors().Values)
            {
                // check if they are either IUpdateableActor or IDrawableActor
                // if yes add them to the updateQueue.
                if (Utils.IsInterfaceImplemented<IUpdateableActor>(actor) ||
                    Utils.IsInterfaceImplemented<IDrawableActor>(actor))
                {
                    updateQueue.Add(actor);
                }
                else if (Utils.IsInterfaceImplemented<IUIActor>(actor))
                {
                    // otherwise add them to the updateUIQueue as long as they
                    // are of the interface IUIActor
                    updateUIQueue.Add(actor);
                }
            }

            // check if there are any actors in the queue.
            if (updateQueue.Count > 0)
            {
                foreach (Actor actor in updateQueue)
                {
                    if (actor as IDrawableActor != null)
                    {
                        // cast them to the IDrawableActor and 
                        // finally update the current actor.
                        IDrawableActor actorx = actor as IDrawableActor;
                        actorx.Update(gameTime);
                    }
                    else if (actor as IUpdateableActor != null)
                    {
                        // same for the IUpdateActor
                        IUpdateableActor actorx = actor as IUpdateableActor;
                        actorx.Update(gameTime);
                    }
                }

                // now clear the queue.
                updateQueue.Clear();
            }

            // update the event manager thus we
            // can fire every event in the list!
            EventManager.Instance.Update();

            // will update all the UIActors such as 
            // Buttons, TextElements and so on.
            if (updateUIQueue.Count > 0)
            {
                foreach (Actor actor in updateUIQueue)
                {
                    IUIActor actorx = actor as IUIActor;
                    actorx.Update(gameTime);
                }

                updateUIQueue.Clear();
            }
        }

        /// <summary>
        /// This method will render all the actors in our world manager
        /// and as it is the case with the update routine will sort them
        /// by their inherited interfaces as well.
        /// </summary>
        /// <param name="gameTime">The current gameTime object from the base game class</param>
        public void Render(GameTime gameTime)
        {
            // Add all the IDrawable and IUIActor instances to our
            // drawQueue and drawUIQueue
            foreach (Actor actor in GetActors().Values)
            {
                if (Utils.IsInterfaceImplemented<IDrawableActor>(actor))
                {
                    drawQueue.Add(actor);
                }
                else if (Utils.IsInterfaceImplemented<IUIActor>(actor))
                {
                    drawUIQueue.Add(actor);
                }
            }

            // check if there are any IDrawableActors
            // if yes just draw them!
            if (drawQueue.Count > 0)
            {
                foreach (Actor actor in drawQueue)
                {
                    IDrawableActor actorx = actor as IDrawableActor;
                    if(actorx.IsVisible)
                        actorx.Render(gameTime);
                }

                // clear the temporary queue 
                // again.
                drawQueue.Clear();
            }

            // check if there are any IUIActors 
            // if yes draw them!
            if (drawUIQueue.Count > 0)
            {
                foreach (Actor actor in drawUIQueue)
                {
                    IUIActor actorx = actor as IUIActor;
                    if (actorx.IsVisible)
                        actorx.Render(gameTime);
                }

                // clear the queue as well.
                drawUIQueue.Clear();
            }
        }
    }
}
