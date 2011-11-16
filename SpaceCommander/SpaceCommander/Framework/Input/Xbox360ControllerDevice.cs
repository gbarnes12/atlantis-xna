using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameApplication.Input
{
    public class Xbox360ControllerDevice : InputDevice<GamePadState>
    {
        #region Private

        private GamePadState current;
        private GamePadState last;

        #endregion

        #region Public

        /// <summary>
        /// returns the value of the left Thumbstick as Vector2
        /// </summary>
        public Vector2 leftStickVector
        {
            get
            {
                return current.ThumbSticks.Left;
            }
        }

        /// <summary>
        /// returns the value of the right Thumbstick as Vector2
        /// </summary>
        public Vector2 rightStickVector
        {
            get
            {
                return current.ThumbSticks.Right;
            }
        }

        /// <summary>
        /// returns the value of the DPad as Vector2
        /// </summary>
        public Vector2 dPadVector
        {
            get
            {
                float x = 0, y = 0;
                if (current.DPad.Left == ButtonState.Pressed)
                    x = -1;
                else if (current.DPad.Right == ButtonState.Pressed)
                    x = 1;
                if (current.DPad.Up == ButtonState.Pressed)
                    y = -1;
                else if (current.DPad.Down == ButtonState.Pressed)
                    y = 1;

                return new Vector2(x, y);
            }
        }

        public override GamePadState State
        {
            get { return current; }
        }

        #endregion


        private static Xbox360ControllerDevice instance;

        public static Xbox360ControllerDevice Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Xbox360ControllerDevice();
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor; creates a new instance of Xbox360ControllerDevice
        /// </summary>
        public Xbox360ControllerDevice()
        {
            current = GamePad.GetState(PlayerIndex.One);
            Update();
        }


        public void Update()
        {
            last = current;

            current = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// button is currently down
        /// </summary>
        /// <param name="b">button to check</param>
        /// <returns></returns>
        public bool isButtonPressed(Buttons b)
        {
            return current.IsButtonDown(b);
        }

        /// <summary>
        /// button is currently up
        /// </summary>
        /// <param name="b">button to check</param>
        /// <returns></returns>
        public bool isButtonUp(Buttons b)
        {
            return current.IsButtonUp(b);
        }

        /// <summary>
        /// button was released the first time
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool wasButtonReleased(Buttons b)
        {
            if (last.IsButtonDown(b) && current.IsButtonUp(b))
                return true;
            return false;
        }

        /// <summary>
        /// button has been down for more than one frame
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool wasButtonHeld(Buttons b)
        {
            if (last.IsButtonDown(b) && current.IsButtonDown(b))
                return true;
            return false;
        }
    }
}
