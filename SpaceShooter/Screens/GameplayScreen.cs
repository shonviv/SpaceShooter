/*
 * Author: Shon Vivier
 * File Name: GameplayScreen.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: Handles all updates and drawing for generic gameplay processes
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ShapeBlaster;

namespace SpaceShooter.Screens
{
    class GameplayScreen
    {
        /// <summary>
        /// The constructor initializes all the base gameplay
        /// </summary>
        public GameplayScreen()
        {
            EntityManager.Add(PlayerShip.Instance);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Sound.Music);
        }

        /// <summary>
        /// Updates all classes regarding gameplay processes
        /// </summary>
        public void Update()
        {
            // Update the HandleInput so that the HandleInput state doesn't bleed into the base update loop
            // I.E pressing 1 in the main menu screen leads to updating the mode selection screen
            // with unupdated HandleInputs meaning that 1 is still perceived to be pressed
            HandleInput.Update();
            
            // Update all gameplay processes
            PlayerStatus.Update();
            EntityManager.Update();
            EnemySpawner.Update();
            GameBase.ParticleManager.Update();
        }

        /// <summary>
        /// Handles drawing the mode selection screen
        /// </summary>
        /// <param name="spriteBatch">Allows graphics to be drawn</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw all the entities on the screen
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();

            // Draw all particles
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            GameBase.ParticleManager.Draw(spriteBatch);
            spriteBatch.End();
            
            // Draw the user interface
            spriteBatch.Begin();

            // Draw the lives, score, and multiplier at the top of the screen
            spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
            DrawRightAlignedString("High Score: " + PlayerStatus.HighScore, 5, spriteBatch);
            DrawRightAlignedString("Score: " + PlayerStatus.Score, 30, spriteBatch);
            DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 55, spriteBatch);
            
            // Draw the custom mouse cursor
            spriteBatch.Draw(Art.Pointer, HandleInput.MousePosition, Color.White);

            // Check to see if the game is over
            if (PlayerStatus.IsGameOver)
            {
                // Draw a simple game over screen
                string text = "Game Over\n" +
                    "Your Score: " + PlayerStatus.Score + "\n" +
                    "High Score: " + PlayerStatus.HighScore + "\n" +
                    "Please wait while we\nreturn you to the menu...";

                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, GameBase.ScreenSize / 2 - textSize / 2, Color.White);
            }

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