using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    public class InputManager
    {
        private KeyboardState _previousState;

        public Vector2 GetMovementDirection()
        {
            KeyboardState currentState = Keyboard.GetState();
            Vector2 direction = Vector2.Zero;

            // Frog moves forward, backward, left and right
            if (IsKeyPressed(Keys.Up, Keys.W, currentState)) direction.Y = -1;
            else if (IsKeyPressed(Keys.Down, Keys.S, currentState)) direction.Y = 1;
            else if (IsKeyPressed(Keys.Left, Keys.A, currentState)) direction.X = -1;
            else if (IsKeyPressed(Keys.Right, Keys.D, currentState)) direction.X = 1;

            _previousState = currentState;
            return direction;
        }

        private bool IsKeyPressed(Keys primary, Keys secondary, KeyboardState current)
        {
            bool isPrimary = current.IsKeyDown(primary) && !_previousState.IsKeyDown(primary);
            bool isSecondary = current.IsKeyDown(secondary) && !_previousState.IsKeyDown(secondary);
            return isPrimary || isSecondary;
        }
    }
}
