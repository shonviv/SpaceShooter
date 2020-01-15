/*
 * Author: Shon Vivier
 * File Name: EntityManager.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: Handles loading all sound content
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
    static class EntityManager
    {
        // Stores a list of all the entities
        static List<Entity> entities = new List<Entity>();

        // Store all of the bullets and enemies in a list
        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();

        // Stores all the added entities
        static List<Entity> addedEntities = new List<Entity>();

        // Returns a count of all the entities
        public static int Count { get { return entities.Count; } }
        
        // Returns a count of all the meteors
        public static int LargeMeteorCount { get { return enemies.Count(enemy => enemy.Type == Enemy.EnemyType.LargeMeteor); } }

        public static int MiddleMeteorCount { get { return enemies.Count(enemy => enemy.Type == Enemy.EnemyType.MiddleMeteor); } }

        public static int SmallMeteorCount { get { return enemies.Count(enemy => enemy.Type == Enemy.EnemyType.SmallMeteor); } }

        // Field that determines whether or not the entity manager is being updated
        static bool isUpdating;

        public static void Add(Entity entity)
        {
            // If the entity manager is updating, add the entity
            if (!isUpdating)
                AddEntity(entity);

            // Otherwise, add the entity to the already added entities
            else
                addedEntities.Add(entity);
        }

        private static void AddEntity(Entity entity)
        {
            // Add the entity to the entities list
            entities.Add(entity);

            // Also add the entity to the appropriate subentity list
            if (entity is Bullet)
                bullets.Add(entity as Bullet);
            else if (entity is Enemy)
                enemies.Add(entity as Enemy);
        }

        /// <summary>
        /// Handles all updates for the entity manager
        /// </summary>
        public static void Update()
        {
            // Start updating
            isUpdating = true;

            // Handle all collisions
            HandleCollisions();

            // Update each individual entity's behaviour
            foreach (var entity in entities)
                entity.Update();

            // Finish updating
            isUpdating = false;

            // Add each entity in the added entities queue
            foreach (var entity in addedEntities)
                AddEntity(entity);

            // Clear the added entities
            addedEntities.Clear();

            // Remove all expired entities from each entity list
            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
        }

        /// <summary>
        /// Handles all collisions between entities
        /// </summary>
        static void HandleCollisions()
        {
            // Handle collisions between enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (IsColliding(enemies[i], enemies[j]))
                    {
                        enemies[i].HandleCollision(enemies[j]);
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            // Handle collisions between bullets and enemies
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(enemies[i], bullets[j]))
                    {
                        // Shoot the enemy
                        enemies[i].WasShot();

                        // Remove the bullet
                        bullets[j].IsExpired = true;
                    }
                }

            // Handle collisions between the player and enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
                {
                    // Kill the player
                    KillPlayer();
                    break;
                }
            }
        }

        /// <summary>
        /// Manages killing the player
        /// </summary>
        private static void KillPlayer()
        {
            // Kills the player ship and removes each enemy
            PlayerShip.Instance.Kill();
            enemies.ForEach(x => x.KillEnemy(true));
            EnemySpawner.Reset();
        }

        /// <summary>
        /// Check to see if entity a is colliding with entity b
        /// </summary>
        private static bool IsColliding(Entity a, Entity b)
        {
            // Get the total radius
            float radius = a.Radius + b.Radius;

            // Check to see if both are not expired and then check to see if the entities are inside of each other
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
        }

        /// <summary>
        /// Returns all nearby entities around a radius at a specific point
        /// </summary>
        /// <param name="position">Takes in a root position</param>
        /// <param name="radius">The searching radius around an initial position</param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        {
            return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
        }

        /// <summary>
        /// Draws all the entities
        /// </summary>
        /// <param name="spriteBatch"></param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }

        /// <summary>
        /// Handles reseting the entity manager
        /// </summary>
        public static void Reset()
        {
            // Wipes all the entity lists
            entities = new List<Entity>();
            enemies = new List<Enemy>();
            bullets = new List<Bullet>();
            addedEntities = new List<Entity>();
        }
    }
}