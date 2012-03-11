namespace GameApplicationTools.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    using Microsoft.Xna.Framework;

    using Events;
    using Interfaces;
    using Interfaces.Collections;
    using Actors.Properties;
    using Microsoft.Xna.Framework.Graphics;


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
         #if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("The ID of the Game View")]
        #endif
        public String GameViewID { get; set; }

        /// <summary>
        /// The collection of actors of this parent node. 
        /// You can add some actors to the collection to
        /// connect them to this actor. 
        /// </summary>
#if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("Collection of Children.")]
#endif
        public virtual ActorNodeCollection Children
        {
            get { return children; }
        }

        /// <summary>
        /// The current parent node of the actor.
        /// </summary>
#if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("The parent actor of this node.")]
#endif
        public virtual Actor Parent
        {
            get;
            set;
        }

        /// <summary>
        /// The position in the world (not relative to the parent node)
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The absolute position within the world.")]
#endif
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// The relative position to the parent node!
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The relative position to the actor's parent.")]
#endif
        public virtual Vector3 Offset { get; set; }


        /// <summary>
        /// Points to the direction away from the vector
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("Forward direction of this actor")]
#endif
        public  Vector3 Forward = Vector3.Forward;


        /// <summary>
        /// Shows the direction in which is up!
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("Up direction of this actor!!")]
#endif
        public  Vector3 Up = Vector3.Up;

        /// <summary>
        /// Rotation of this node represented as 
        /// Quaternion.
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The Rotation of this Actor. Shouldn't be modified by hand!")]
#endif
        public virtual Quaternion Rotation { get; set; }

        /// <summary>
        /// Determines if this actor is visible or not
        /// </summary>
#if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("Is this actor visible?")]
#endif
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
#if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("Does this actor get updated?")]
#endif
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
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The bounding sphere which is used for collision")]
#endif
        public virtual BoundingSphere BoundingSphere
        {
            get { return GetBoundingSphere(); }
        }

        /// <summary>
        /// This is the world matrix but is transformed
        /// by the scene graph and the parent nodes. 
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The absolute transformation by the world.")]
#endif
        public virtual Matrix AbsoluteTransform { get; set; }

        /// <summary>
        /// You have the possibility to hook 
        /// an controller to the actor if you want. 
        /// 
        /// This could be something like a path finding
        /// or rotation controller. 
        /// </summary>
#if !XBOX360
        [CategoryAttribute("General"), DescriptionAttribute("You can hook a controller to this actor if you want to!")]
#endif
        public virtual IController Controller { get; set; }

        /// <summary>
        /// The scale of the object.
        /// </summary>
#if !XBOX360
        [CategoryAttribute("World"), DescriptionAttribute("The scale of the actor.")]
#endif
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
        /// Get the BoundingBox 
        /// </summary>
        /// <returns>Return a valid bounding box <see cref="BoundingBox" /></returns>
        public virtual BoundingBox GetBoundingBox()
        {
            return new BoundingBox(Position - (Vector3.One) * Scale, Position + (Vector3.One) * Scale); 
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
        public virtual void PreRender() { GameApplication.Instance.GetGraphics().DepthStencilState = DepthStencilState.Default; }

        /// <summary>
        /// The render method. This just does nothing!
        /// </summary>
        /// <param name="sceneGraph">The scene graph responsible for this actor - <see cref="SceneGraphManager"/></param>
        public virtual void Render(SceneGraphManager sceneGraph) { }

        /// <summary>
        /// The post render method. This just does nothing!
        /// </summary>
        public virtual void PostRender() { }
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
