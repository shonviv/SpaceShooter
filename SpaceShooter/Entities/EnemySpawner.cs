/*
 * Author: Shon Vivier
 * File Name: EnemySpawner.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: Responsible for the spawning of enemies
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    static class EnemySpawner
    {
        // Define a new random
        static Random rand = new Random();

        // Set an inverse spawn chance that decreases with time
        static float inverseSpawnChance = 90;
        static float maxInverseSpawnChance = 30;
        static float spawnChanceIncreaser = 0.005f;

        // Define the time until next meteor should spawn and the time currently elapsed
        static float meteorTimer = 0;
        static float maxMeteorTimer = 120;

        // Define the min position the player can be away from the random spawn
        static float minPosition = 250 * 500;

        /// <summary>
        /// Handles updating the enemy spawning
        /// </summary>
        public static void Update()
        {
            // Update based on the game state
            // Each game state has a different enemy spawning behaviour
            switch (GameBase.State)
            {
                case GameBase.GameState.ClassicGameplay:
                    ClassicSpawner();
                    break;
                case GameBase.GameState.FreeGameplay:
                    FreeSpawner();
                    break;
            }
        }

        /// <summary>
        /// Handles the spawning of large meteors in the classic mode
        /// </summary>
        private static void ClassicSpawner()
        {
            // Increment the meteor timer
            meteorTimer++;

            // Check to see if it is time to spawn the next meteor
            // Also check if there currently exist less than 3 large meteors, 4 middle, and 10 small and the ship is not dead
            if (meteorTimer >= maxMeteorTimer && EntityManager.LargeMeteorCount < 3 && EntityManager.MiddleMeteorCount <= 4
                 && EntityManager.SmallMeteorCount <= 10 && !PlayerShip.Instance.IsDead)
            {
                // Select a random position just off screen
                Vector2 RandomPosition = new Vector2(rand.Next(0, (int)GameBase.ScreenSize.X), rand.Next((int)-GameBase.ScreenSize.Y / 4, 0));

                // Add the meteor to the entity manager and reset the meteor timer
                EntityManager.Add(Enemy.LargeMetoer(RandomPosition));
                meteorTimer = 0;
            }
        }

        /// <summary>
        /// Handles spawning in the free game mode
        /// </summary>
        private static void FreeSpawner()
        {
            // Check to see if the ship is not dead and if less than the max number of free entities exist
            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
            {
                // Spawn the next enemy according to the spawn chance
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
            }

            // Slowly increase the spawn rate as time progresses
            if (inverseSpawnChance > maxInverseSpawnChance)
                inverseSpawnChance -= spawnChanceIncreaser;
        }

        /// <summary>
        /// Returns a spawn position away from the player
        /// </summary>
        /// <returns>A Vector2 corresponding to the spawn position</returns>
        private static Vector2 GetSpawnPosition()
        {
            // Create a new position vector
            Vector2 pos;
            do
            {
                // Get a random position between the screen dimensions
                pos = new Vector2(rand.Next((int)GameBase.ScreenSize.X), rand.Next((int)GameBase.ScreenSize.Y));
            }
            
            // Repeat this until a distance away from the player is given
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < minPosition);

            // Return the position
            return pos;
        }

        /// <summary>
        /// Resets the enemy spawners
        /// </summary>
        public static void Reset()
        {
            // Sets the inverse spawn chance to the default value
            inverseSpawnChance = 50;
        }
    }
}
