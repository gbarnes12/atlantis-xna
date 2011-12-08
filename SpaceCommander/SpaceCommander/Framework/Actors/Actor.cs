namespace GameApplicationTools.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Events;
    using Interfaces;
    using GameApplicationTools.Interfaces.Collections;
    using Microsoft.Xna.Framework;
    using GameApplicationTools.Actors.Properties;
using System.ComponentModel;

    /// <summary>
    /// This is the basic actor class from
    /// which you need to inherit if you create
    /// any actor. It only holds the ID of the 
    /// Actor and inherits from the EventListener
    /// class due to the fact that every actor can
    /// be a EventListener.
    /// 
    /// Example (How to Register to an Event):
    /// // this represents the Constructor
    /// public ExampleActor() {
    ///     this.RegisterEvent(ID, EventType.KeyboardEvent_KeyWasPressed, "KeyboardKeyWasPressedEventListener");
    /// }
    /// 
    /// and of course you need to create the specified method!
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Actor : EventListener, IActorNode
    {
        #region Private
        private ActorNodeCollection children;
        private bool _visible;
        private bool _updateable;
        #endregion

        #region Public
        /// <summary>
        /// Returns or sets the current ID 
        /// of this Actor. It needs to be unique
        /// otherwise the WorldManager will prompt
        /// some error. 
        /// </summary>
        #if !XBOX360
        [CategoryAttribute("General"),DescriptionAttribute("The ID of this Actor!")]
        #endif
        public String ID { get; set; }

        /// <summary>
        /// Returns or sets the current ID of the GameView
        /// in which this Actors was created. This will be 
        /// an unique ID and can be used to retrieve this
        /// actor by its GV ID.
        /// </summary>
        public String GameViewID { get; set; }

        /// <summary>
        /// The collection of actors of this parent node. 
        /// You can add some actors to the collection to
        /// connect them to this actor. 
        /// </summary>
        public virtual ActorNodeCollection Children
        {
            get { return children; }
        }

        /// <summary>
        /// The current parent node of the actor.
        /// </summary>
        public virtual Actor Parent
        {
            get;
            set;
        }

        /// <summary>
        /// The position in the world (not relative to the parent node)
        /// </summary>
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// The relative position to the parent node!
        /// </summary>
        public virtual Vector3 Offset { get; set; }

        /// <summary>
        /// Rotation of this node represented as 
        /// Quaternion.
        /// </summary>
        public virtual Quaternion Rotation { get; set; }

        /// <summary>
        /// Determines if this actor is visible or not
        /// </summary>
        public bool Visible 
        {
            get
            {
                return _visible;
            }
            set
            {
                SetVisibilityNodes(value);
            }
        }

        /// <summary>
        /// Determines if this actor gets updated or 
        /// not.
        /// </summary>
        public bool Updateable
        {
            get
            {
                return _updateable;
            }
            set
            {
                SetUpdateabilityNodes(value);
            }
        }
        /// <summary>
        /// Returns the bounding sphere which was calculated 
        /// by the scene graph.
        /// </summary>
        public virtual BoundingSphere BoundingSphere
        {
            get { return GetBoundingSphere(); }
        }

        /// <summary>
        /// This is the world matrix but is transformed
        /// by the scene graph and the parent nodes. 
        /// </summary>
        public virtual Matrix AbsoluteTransform { get; set; }

        /// <summary>
        /// You have the possibility to hook 
        /// an controller to the actor if you want. 
        /// 
        /// This could be something like a path finding
        /// or rotation controller. 
        /// </summary>
        public virtual IController Controller { get; set; }

        /// <summary>
        /// The scale of the object.
        /// </summary>
        public virtual Vector3 Scale { get; set; }
        
        /// <summary>
        /// An actor can contain various of different properties 
        /// thus we aren't limited anymore and we can use for example
        /// the mesh object to represent every possible state.
        /// </summary>
        public Dictionary<ActorPropertyType, ActorProperty> Properties;
        #endregion

        /// <summary>
        /// Will set the ID of this actor and 
        /// needs to be called in every class which
        /// inherits from this.
        /// </summary>
        /// <param name="ID">The Actor-ID this needs to be unique.</param>
        public Actor(String ID, String GameViewID)
        {
            // generate the ID
            this.ID = ID;
            this.GameViewID = GameViewID;
            this.Position = Vector3.Zero;
            this.Offset = Vector3.Zero;
            this.Rotation = Quaternion.Identity;
            this.Visible = true;
            this.Updateable = true;
            this.AbsoluteTransform = Matrix.Identity;
            this.Parent = null;
            this.Scale = new Vector3(1);
            children = new ActorNodeCollection(this);
            Properties = new Dictionary<ActorPropertyType, ActorProperty>();
        }

        /// <summary>
        /// Get the BoundingSphere with the value of Vector3.Zero and 
        /// a radius of 0.
        /// </summary>
        /// <returns>Return a valid bounding sphere <see cref="BoundSphere" /></returns>
        public virtual BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Vector3.Zero, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetVisibilityNodes(bool value)
        {
            _visible = value;

            if (Children != null)
            {
                foreach (Actor actor in Children)
                    actor.Visible = value;
            } 
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void SetUpdateabilityNodes(bool value)
        {
            _updateable = value;

            if (Children != null)
            {
                foreach (Actor actor in Children)
                    actor.Updateable = value;
            }
        }

        /// <summary>
        /// The load content method. This just does nothing!
        /// </summary>
        public virtual void LoadContent(){}

        /// <summary>
        /// The update method. This just does nothing!
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public virtual void Update(SceneGraphManager sceneGraph)
        {
            if (Controller != null)
                Controller.UpdateSceneNode(this, sceneGraph.GameTime);
        }

        /// <summary>
        /// The pre render method. This just does nothing!
        /// </summary>
        public virtual void PreRender() { }

        /// <summary>
        /// The render method. This just does nothing!
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public virtual void Render(SceneGraphManager sceneGraph) { }
    }

    /// <summary>
    /// Represents the EventListener which
    /// is only for an actor because we build 
    /// the whole EventManger upon our WorldManager
    /// System.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class EventListener
    {
        #region Private
        // the methods of every EventType you need to create
        Dictionary<EventType, List<string>> EventMethods;
        #endregion

        /// <summary>
        /// Just creates the instance of the EventMethod
        /// dictionary.
        /// </summary>
        public EventListener()
        {
            EventMethods = new Dictionary<EventType, List<string>>();
        }

        /// <summary>
        /// Registers an event at the event manager and its methods
        /// within the listener
        /// </summary>
        /// <param name="ID">The ID of the Actor or GameView</param>
        /// <param name="Event">The specific event which should get registrated!</param>
        /// <param name="Method"></param>
        protected void RegisterEvent(String ID, EventType Event, string Method)
        {
            if (!EventMethods.ContainsKey(Event))
                EventMethods.Add(Event, new List<string>());

            EventMethods[Event].Add(Method); 

            // register this event for the listener at the
            // event manager
            EventManager.Instance.SubscribeEvent(Event, ID);
        }

        /// <summary>
        /// This method needs to be integrated into the main actor class.
        /// </summary>
        /// <param name="Event"></param>
        /// <returns>a bool value which tells the EventManager if the Event was successful or not!</returns>
        public virtual bool ProcessEvents(EventType EventType, Event Event) 
        {
            List<string> methods = EventMethods[EventType];
            bool Return = true; 

            foreach (string method in methods)
            {
                object[] arguments = {Event};
                Return = (bool)this.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, this, arguments);
            }

            return Return; 
        }
    }
}
