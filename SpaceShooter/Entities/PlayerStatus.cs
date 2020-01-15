/*
 * Author: Shon Vivier
 * File Name: PlayerStatus.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/20/2019
 * Description: Handles all the values the player is responsible for
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShapeBlaster
{
    static class PlayerStatus
    {
        // Amount of time it takes in seconds for a multiplier to expire
        private const float multiplierExpiryTime = 0.8f;

        // The maximum multiplier amount
        private const int maxMultiplier = 20;

        // The player properties
        public static int Lives { get; private set; }

        public static int Score { get; private set; }

        public static int HighScore { get; private set; }

        public static int Multiplier { get; private set; }

        public static bool IsGameOver { get { return Lives == 0; } }

        // Defines how much time is left before hte multiplier expires
        private static float multiplierTimeLeft;

        // Defines the highscore file name
        private const string highScoreFilename = "highscore.txt";

        /// <summary>
        /// Initializes the player information
        /// </summary>
        static PlayerStatus()
        {
            // Loads the highscore and resets the player data
            HighScore = LoadHighScore();
            Reset();
        }

        /// <summary>
        /// Resets all the player properties
        /// </summary>
        public static void Reset()
        {
            // Save the highscore if the current score is greater than the highscore
            if (Score > HighScore)
                SaveHighScore(HighScore = Score);

            // Load the high score and reset the player data
            HighScore = LoadHighScore();
            Score = 0;
            Multiplier = 1;
            Lives = 3;
            multiplierTimeLeft = 0;
        }

        /// <summary>
        /// Handles the update loop for the player status
        /// </summary>
        public static void Update()
        {
            // Check to see if the score is greater than or equal to the high score
            if (Score >= HighScore)
            {
                // Set the high score as the current score
                HighScore = Score;

                // If the game is over, save the high score
                if (IsGameOver)
                {
                    SaveHighScore(HighScore);
                }
            }
            
            // Check to see if the multiplier is greater than 1
            if (Multiplier > 1)
            {
                // Update the multiplier timer
                if ((multiplierTimeLeft -= (float)GameBase.GameTime.ElapsedGameTime.TotalSeconds) <= 0)
                {
                    // Reset the multiplier time if the multiplier time expires
                    multiplierTimeLeft = multiplierExpiryTime;
                    ResetMultiplier();
                }
            }
        }

        /// <summary>
        /// Adds points to the player's score
        /// </summary>
        /// <param name="basePoints">How many points the player would receive before a multiplier applied</param>
        public static void AddPoints(int basePoints)
        {
            // Do not add any points if the ship is dead
            if (PlayerShip.Instance.IsDead)
                return;

            // Add the base amount of points times the multiplier to the score
            Score += basePoints * Multiplier;
        }

        /// <summary>
        /// Increases the multiplier for the player
        /// </summary>
        public static void IncreaseMultiplier()
        {
            // Do not increase the multiplier if the ship is dead
            if (PlayerShip.Instance.IsDead)
                return;

            // Set the multiplier time left to the multiplier expiry time and add the multiplier if it is less than the max multiplier
            multiplierTimeLeft = multiplierExpiryTime;
            if (Multiplier < maxMultiplier)
                Multiplier++;
        }

        /// <summary>
        /// Resets the multiplier to 1
        /// </summary>
        public static void ResetMultiplier()
        {
            Multiplier = 1;
        }

        /// <summary>
        /// Removes 1 life from the player
        /// </summary>
        public static void RemoveLife()
        {
            Lives--;
        }

        /// <summary>
        /// Loads all high scores
        /// </summary>
        /// <returns></returns>
        private static int LoadHighScore()
        {
            // Declare a score value and set the intial game index to 0 (classic)
            int score;
            int gameIndex = 0;

            // Check to see if the game state is not classic and adjust the game index accordingly
            // Classic = 0, Free = 1
            switch (GameBase.State)
            {
                case GameBase.GameState.FreeGameplay:
                    gameIndex = 1;
                    break;
            }

            // Check to see if the high score file exists
            if (File.Exists(highScoreFilename))
            {
                // Read the high score for the corresponding game
                int.TryParse(File.ReadAllLines(highScoreFilename)[gameIndex], out score);
            }
            else
            {
                // If the file doesn't exist, the default score is 0
                return 0;
            }

            // Return the score
            return score;
        }

        /// <summary>
        /// Saves the newly made high score
        /// </summary>
        /// <param name="score">The score to save</param>
        private static void SaveHighScore(int score)
        {
            // Define and set the intial game index to 0 (classic)
            int gameIndex = 0;

            // Get a list of all the high scores
            List<string> highscores = File.ReadAllLines(highScoreFilename).ToList();

            // Check to see if the game state is not classic and adjust the game index accordingly
            // Classic = 0, Free = 1
            switch (GameBase.State)
            {
                case GameBase.GameState.FreeGameplay:
                    gameIndex = 1;
                    break;
            }

            // Replace the score at the game index with the new score
            highscores[gameIndex] = score.ToString();

            // Write it back into the file
            File.WriteAllLines(highScoreFilename, highscores);
        }
    }
}
