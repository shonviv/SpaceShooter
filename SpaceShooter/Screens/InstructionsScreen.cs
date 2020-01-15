/*
 * Author: Shon Vivier
 * File Name: InstructionsScreen.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: Handles all updates and drawing for the instructions screen
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeBlaster;

namespace SpaceShooter.Screens
{
    public static class InstructionsScreen
    {
        /// <summary>
        /// Handles updating the mode selection screen
        /// </summary>
        public static void Update()
        {
            // Update the HandleInput so that the HandleInput state doesn't bleed into the base update loop
            // I.E pressing 1 in the main menu screen leads to updating the mode selection screen
            // with unupdated HandleInputs meaning that 1 is still perceived to be pressed
            HandleInput.Update();

            // Check if 1 was pressed
            if (HandleInput.WasKeyPressed(Keys.D1))
                // Update the scene to the main menu screen
                GameBase.State = GameBase.GameState.MainMenu;
        }

        /// <summary>
        /// Handles drawing the instructions screen
        /// </summary>
        /// <param name="spriteBatch">Allows graphics to be drawn</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            // Begin drawing
            spriteBatch.Begin();

            // Draw the custom mouse cursor
            spriteBatch.Draw(Art.Pointer, HandleInput.MousePosition, Color.White);

            // Define text to draw
            string text = "Instructions\n\n" +
                "Shoot the balls\n" +
                "Don't get hit\n" +
                "Move with WASD\n" +
                "Survive...\n\n" +
                "1.Back";

            // Measure the text size and draw the text aligned to the right
            Vector2 textSize = Art.Font.MeasureString(text);
            spriteBatch.DrawString(Art.Font, text, GameBase.ScreenSize / 2 - textSize / 2, Color.White);

            // Stop drawing
            spriteBatch.End();
        }

        /// <summary>
        /// This method draws strings aligned to the right
        /// </summary>
        /// <param name="text">The text to be drawn</param>
        /// <param name="y">The position on the y-axis</param>
        /// <param name="spriteBatch">Allows graphics to be drawn</param>
        private static void DrawRightAlignedString(string text, float y, SpriteBatch spriteBatch)
        {
            // Measure the width of the text
            var textWidth = Art.Font.MeasureString(text).X;

            // Draw it according to the y-axis aligned to the right
            spriteBatch.DrawString(Art.Font, text, new Vector2(GameBase.ScreenSize.X - textWidth - 5, y), Color.White);
        }
    }
}
