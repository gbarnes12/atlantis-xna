namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Actors;
    using Events;

    /// <summary>
    /// The EventManager handles all our 
    /// events as the name states this matter of fact
    /// already. You can either register a listener to 
    /// a specific EventType or can hook any Event you
    /// like to all its listeners. 
    /// 
    /// Example (Hook of an Event): 
    /// 
    /// Event event = new Event(); 
    /// event.data = data;
    /// EventManager.Instance.HookEvent(EventType, Event);
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class EventManager
    {
        #region Public
        /// <summary>
        /// Maintains a list to our event listeners
        /// </summary>
        public Dictionary<EventType, List<String>> Listeners;

        /// <summary>
        /// Main queue in which we will push the event hooks from
        /// our sender's 
        /// </summary>
        public Dictionary<EventType, List<Event>> Queue;
        #endregion

        #region Private
        private static EventManager instance;
        #endregion

        /// <summary>
        /// Returns the singleton instance of the EventManager
        /// </summary>
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }

        public EventManager()
        {
            Listeners = new Dictionary<EventType, List<String>>();
            Queue = new Dictionary<EventType, List<Event>>();
        }

        /// <summary>
        /// Creates a new event.
        /// </summary>
        /// <typeparam name="T">Event</typeparam>
        /// <returns>T - Event</returns>
        public Event CreateEvent()
        {
            return new Event();
        }

        /// <summary>
        /// Subscribes an event listener to the event 
        /// listeners list.
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="ID"></param>
        public void SubscribeEvent(EventType Event, String ID)
        {
            if (!Listeners.ContainsKey(Event))
                Listeners.Add(Event, new List<String>());

            if (!Listeners[Event].Contains(ID))
                Listeners[Event].Add(ID); 
        }

        /// <summary>
        /// Removes an event listener on the 
        /// listener list. 
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="ID"></param>
        public void UnsubscribeEvent(EventType EventType, String ID)
        {
            List<String> listeners = this.Listeners[EventType];

            int i = 0;
            foreach (String id in listeners)
            {
                if (id == ID)
                    listeners.RemoveAt(i);

                i++;
            }
        }

        /// <summary>
        /// Hooks an event from any sender to the queue!
        /// </summary>
        /// <param name="EventType"></param>
        /// <param name="Event"></param>
        public void HookEvent(EventType EventType, Event Event)
        {
            if (Queue != null)
            {
                if (!Queue.ContainsKey(EventType))
                    Queue.Add(EventType, new List<Event>());

                if (!Queue[EventType].Contains(Event))
                    Queue[EventType].Add(Event);
            }
            else
            {
                throw new Exception("Queue hasn't been initialized");
            }
        }

        /// <summary>
        /// Immediately fires a specific event type
        /// to all its listeners
        /// </summary>
        /// <param name="Event"></param>
        public void FireEvent(EventType Event)
        {
            throw new NotImplementedException("This methods needs to be implemented");
        }

        /// <summary>
        /// Updates our current event queue 
        /// and swaps the waiting queue
        /// </summary>
        public void Update()
        {
            Dictionary<EventType, List<Event>> CurrentQueue = new Dictionary<EventType, List<Event>>(Queue);
            Queue.Clear();

            if (CurrentQueue.Count > 0)
            {
                foreach (EventType EventType in CurrentQueue.Keys)
                {
                    // go through our listeners
                    if (Listeners.ContainsKey(EventType))
                    {
                        List<String> listenersCurrent = Listeners[EventType];
                        if (listenersCurrent.Count > 0)
                        {
                            foreach (String ListenerID in listenersCurrent)
                            {
                                if (CurrentQueue.ContainsKey(EventType))
                                {
                                    List<Event> Events = CurrentQueue[EventType];
                                    if (Events.Count > 0)
                                    {
                                        //go through our events
                                        foreach (Event Event in Events)
                                        {
                                            //set listener id
                                            Event.Listener = ListenerID;

                                            if (!WorldManager.Instance.GetActor(ListenerID).ProcessEvents(EventType, Event) && Event.HookAgain)
                                                this.HookEvent(EventType, Event); // if we failed for some reason we need to send it back to the queue
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                CurrentQueue.Clear();
            }
        }

    }
}
