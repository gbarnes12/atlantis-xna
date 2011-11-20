namespace GameApplicationTools.Actors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Events;

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
    public class Actor : EventListener
    {
        #region Public
        /// <summary>
        /// Returns or sets the current ID 
        /// of this Actor. It needs to be unique
        /// otherwise the WorldManager will prompt
        /// some error. 
        /// </summary>
        public String ID { get; set; }

        /// <summary>
        /// Returns or sets the current ID of the GameView
        /// in which this Actors was created. This will be 
        /// an unique ID and can be used to retrieve this
        /// actor by its GV ID.
        /// </summary>
        public String GameViewID { get; set; }
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
        }

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
