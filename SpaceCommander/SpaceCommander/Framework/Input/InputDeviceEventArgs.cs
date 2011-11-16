namespace GameApplication.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    // The base input device class. Other input device types will
    // inherit from it. The <T> generic allows us to specify what
    // type of input device state it manages. (MouseState, etc.)
    public abstract class InputDevice<T>
    {
        // The State object of type T specified above
        public abstract T State { get; }
    }

    // An input device event argument class that can handle events
    // for several types of input device. "O" specified what type
    // of object the event was triggered by (Key, Button,
    // MouseButton, etc). "S" specifies the type of state the
    // event provides (MouseState, KeyboardState, etc.)
    public class InputDeviceEventArgs<O, S> : EventArgs
    {
        // The object of type O that triggered the event
        public O Object;

        // The input device that manages the state of type S that
        // owns the triggered object
        public InputDevice<S> Device;

        // The state of the input device of type S that was triggered
        public S State;

        // Constructor takes the triggered object and input device
        public InputDeviceEventArgs(O Object, InputDevice<S> Device)
        {
            this.Object = Object;
            this.Device = Device;
            this.State = ((InputDevice<S>)Device).State;
        }
    }

    // An input device event handler delegate. This defines what type
    // of method may handle an event. In this case, it is a void that
    // accepts the specified input device arguments
    public delegate void InputEventHandler<O, S>(object sender,
        InputDeviceEventArgs<O, S> e);
}
