namespace GameApplicationTools.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework.Input;

    using Actors;
    using UI;

    /// <summary>
    /// Contains all the event types
    /// we can use within our code.
    /// </summary>
    public enum EventType
    {
        KeyboardEvent_KeyWasPressed,
        KeyboardEvent_KeyWasReleased,
        KeyboardEvent_KeyIsDown,
        ButtonEvent_OnClick
    }

    /// <summary>
    /// Base Event class. This is the event 
    /// class which every other event inherits of. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The Sender object of this specific event
        /// </summary>
        public object Sender { get; set; }
        
        /// <summary>
        /// The Listener of this specific event
        /// </summary>
        public String Listener { get; set; }

        /// <summary>
        /// Tells the event whether it will be
        /// hooked again to the event queue after failure or 
        /// not!
        /// </summary>
        public bool HookAgain { get; set; }

        /// <summary>
        /// Creates the new Event and sets the HookAgain 
        /// value to false.
        /// </summary>
        public Event() { HookAgain = false; }
    }

    public class KeyboardEvent_KeyWasPressed : Event { public Keys Key { get; set; } }

    public class KeyboardEvent_KeyWasReleased : Event { public Keys Key { get; set; } }

    public class ButtonEvent_OnClick : Event { public Button button { get; set; } }
}
