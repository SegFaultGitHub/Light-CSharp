using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcadeGame
{
    class Input
    {
        private static KeyboardState keyboardState_, oldKeyboardState_;
        private static MouseState mouseState_, oldMouseState_;

        public static void Update()
        {
            oldKeyboardState_ = keyboardState_;
            keyboardState_ = Keyboard.GetState();
            oldMouseState_ = mouseState_;
            mouseState_ = Mouse.GetState();
        }

        public static bool IsPressed(Keys key)
        {
            return keyboardState_.IsKeyDown(key);
        }

        public static bool IsPressedOnce(Keys key)
        {
            return keyboardState_.IsKeyDown(key) && oldKeyboardState_.IsKeyUp(key);
        }
    }
}
