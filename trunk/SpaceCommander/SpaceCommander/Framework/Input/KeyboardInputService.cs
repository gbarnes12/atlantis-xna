namespace GameApplicationTools.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UI;
    using Microsoft.Xna.Framework.Input;

    public class KeyboardInputService : GameConsole.IKeyboardInputService
    {
        public KeyboardState currentKeyboard, previousKeyboard;

        #region XConsole.IKeyboardInputService Members

        public bool IsJustPressed(Keys key)
        {
            return currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        public bool IsDown(Keys key)
        {
            return currentKeyboard.IsKeyDown(key);
        }

        public Keys[] GetPressedKeys()
        {
            return currentKeyboard.GetPressedKeys();
        }

        #endregion
    }
}
