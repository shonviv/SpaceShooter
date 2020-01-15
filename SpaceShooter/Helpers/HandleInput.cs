/*
 * Author: Shon Vivier
 * File Name: HandleInput.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/18/2019
 * Description: Handles loading all sound content
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    static class HandleInput
    {
        // Store the current and last keyboard states
        private static KeyboardState keyboardState;
        private static KeyboardState lastKeyboardState;

        // Store the current and last mouse states
        private static MouseState mouseState;
        private static MouseState lastMouseState;
        
        // Returns the mouse position
        public static Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

        /// <summary>
        /// Updates the keyboard and mouse states
        /// </summary>
        public static void Update()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }

        /// <summary>
        /// Checks if a key was just pressed down
        /// </summary>
        /// <param name="key">Gets the key to check</param>
        /// <returns></returns>
        public static bool WasKeyPressed(Keys key)
        {
            // Check if the key is up this frame and if the key was down last frame
            return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns the movement direction of the player
        /// </summary>
        /// <returns>Vector2</returns>
        public static Vector2 GetMovementDirection()
        {
            // Define the direction
            Vector2 direction = new Vector2(0);
            
            // Change the X direction bsaed on whether A or D is pressed
            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;

            // If the game is in Free gameplay, allow the player to move up and down in the Y direction
            if (GameBase.State == GameBase.GameState.FreeGameplay)
            {
                if (keyboardState.IsKeyDown(Keys.W))
                    direction.Y -= 1;
                if (keyboardState.IsKeyDown(Keys.S))
                    direction.Y += 1;
            }

            // Clamp the length of the vector to a maximum of 1
            if (direction.LengthSquared() > 1)
                direction.Normalize();

            return direction;
        }
        
        /// <summary>
        /// Returns the mouse aim direction
        /// </summary>
        /// <returns>Vector2</returns>
        public static Vector2 GetMouseAimDirection()
        {
            // Set the direction of fire as the mouse position minues the player ship position
            Vector2 direction = MousePosition - PlayerShip.Instance.Position;

            // If there is no aim HandleInput, return zero. Otherwise, normalize the direction to have a length of 1
            if (direction == Vector2.Zero)
                return Vector2.Zero;
            else
                return Vector2.Normalize(direction);
        }
    }
}
