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
            updateQueue = new List<Actor>();
            drawQueue = new List<Actor>();
            updateUIQueue = new List<Actor>();
            drawUIQueue = new List<Actor>();
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
            foreach (Actor actor in WorldManager.Instance.GetActors().Values)
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
                        if(actorx.IsUpdateable)
                            actorx.Update(gameTime);
                    }
                    else if (actor as IUpdateableActor != null)
                    {
                        // same for the IUpdateActor
                        IUpdateableActor actorx = actor as IUpdateableActor;
                        if (actorx.IsUpdateable)
                            actorx.Update(gameTime);
                    }
                }

                // now clear the queue.
                updateQueue.Clear();
            }

            // will update all the UIActors such as 
            // Buttons, TextElements and so on.
            if (updateUIQueue.Count > 0)
            {
                foreach (Actor actor in updateUIQueue)
                {
                    IUIActor actorx = actor as IUIActor;
                    if (actorx.IsUpdateable)
                        actorx.Update(gameTime);
                }

                updateUIQueue.Clear();
            }


            // update the event manager thus we
            // can fire every event in the list!
            EventManager.Instance.Update();
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
            foreach (Actor actor in WorldManager.Instance.GetActors().Values)
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
                    if (actorx.IsVisible)
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
