/*
 * Author: Shon Vivier
 * File Name: Enemy.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: Handles all enemy operations and behaviour
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
    class Enemy : Entity
    {
        // Create a new random
        public static Random rand = new Random();

        // Define a list of behaviours
        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();

        // Sets the amount of time to wait until updating enemy behaviour
        private int timeUntilStart = 1;

        // Used to determine whether or not the enemy is currently active
        public bool IsActive { get { return timeUntilStart <= 0; } }
        
        // The enemy info
        public int PointValue { get; private set; }

        public int Health { get; private set; }

        // Determines the type of enemy
        public EnemyType Type { get; private set; }

        // An enum storing all the different enemy types
        public enum EnemyType
        {
            Seeker,
            Wanderer,
            LargeMeteor,
            MiddleMeteor,
            SmallMeteor,
        }

        /// <summary>
        /// The default enemy constructor
        /// </summary>
        public Enemy(Texture2D image, Vector2 position)
        {
            // Set the default enemy image and position
            this.image = image;
            Position = position;

            // Save the radius as the width of the image divided by 2
            Radius = image.Width / 2f;

            // Set the default colour to be transparent
            color = Color.Transparent;

            // Set the default point and health values
            PointValue = 1;
            Health = 1;
        }

        /// <summary>
        /// Creates a seeker enemy
        /// </summary>
        public static Enemy CreateSeeker(Vector2 position)
        {
            // Creates a new enemy with seeker type at a specified position
            var enemy = new Enemy(Art.Seeker, position);

            // Adds the enemy behaviours
            enemy.AddBehaviour(enemy.FollowPlayer(9.81f/10));

            // Sets the point value and enemy type
            enemy.PointValue = 100;
            enemy.Type = EnemyType.Seeker;

            // Return the enemy
            return enemy;
        }

        /// <summary>
        /// Creates a wanderer enemy
        /// </summary>
        public static Enemy CreateWanderer(Vector2 position)
        {
            // Creates a new enemy with wanderer type at a specified position
            var enemy = new Enemy(Art.Wanderer, position);

            // Adds the enemy behaviours
            enemy.AddBehaviour(enemy.MoveRandomly());

            // Sets the point value and enemy type
            enemy.PointValue = 50;
            enemy.Type = EnemyType.Wanderer;

            // Return the enemy
            return enemy;
        }

        /// <summary>
        /// Creates a large meteor enemy
        /// </summary>
        public static Enemy LargeMetoer(Vector2 position)
        {
            // Creates a new enemy with wanderer type at a specified position
            var enemy = new Enemy(Art.LargeMeteor, position);

            // Adds the enemy behaviours
            enemy.AddBehaviour(enemy.GravityMove(new Vector2(0, 0)));

            // Sets the health and enemy type
            enemy.Health = 5;
            enemy.PointValue = 500;
            enemy.Type = EnemyType.LargeMeteor;

            // Return the enemy
            return enemy;
        }

        /// <summary>
        /// Creates a middle meteor enemy
        /// </summary>
        public static Enemy MiddleMeteor(Vector2 position, Vector2 velocity)
        {
            // Creates a new enemy with wanderer type at a specified position
            var enemy = new Enemy(Art.MiddleMeteor, position);

            // Adds the enemy behaviours
            enemy.AddBehaviour(enemy.GravityMove(velocity));

            // Sets the health and enemy type
            enemy.Health = 3;
            enemy.PointValue = 250;
            enemy.Type = EnemyType.MiddleMeteor;

            // Return the enemy
            return enemy;
        }

        /// <summary>
        /// Creates a small meteor enemy
        /// </summary>
        public static Enemy SmallMeteor(Vector2 position, Vector2 velocity)
        {
            // Creates a new enemy with wanderer type at a specified position
            var enemy = new Enemy(Art.SmallMeteor, position);

            // Adds the enemy behaviours
            enemy.AddBehaviour(enemy.GravityMove(velocity));

            // Sets the enemy type and point value
            enemy.Type = EnemyType.SmallMeteor;
            enemy.PointValue = 150;

            // Return the enemy
            return enemy;
        }

        /// <summary>
        /// Handles updating the enemy behaviour
        /// </summary>
        public override void Update()
        {
            // Check to see if the enemy is active and apply behaviours if so
            if (timeUntilStart <= 0)
                ApplyBehaviours();
            else
            {
                // Remove the time to start
                timeUntilStart--;

                // Set the colour as white and flash through it depending on the time left to start
                color = Color.White * (1 - timeUntilStart / 60f);
            }

            // Update the position and clamp it to the screen dimensions with some leeway at the top wall
            Position += Velocity;
            Position = Vector2.Clamp(Position, new Vector2(Size.X / 2, -3 * (Size.Y / 2)), GameBase.ScreenSize - Size / 2);
            Velocity *= 0.8f;
        }

        /// <summary>
        /// Draw the enemy textures
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Check to see if the enmy is active
            if (timeUntilStart > 0)
            {
                // Draw an expanding, fading-out version of the sprite as part of the spawn-in effect
                // Decreases from 1 to 0 as the enemy spawns in
                float factor = timeUntilStart / 60f;

                // Draw the enemy
                spriteBatch.Draw(image, Position, null, Color.White * factor, Orientation, Size / 2f, 2 - factor, 0, 0);
            }
            
            // Draws the amount of health remaining on the enemy
            spriteBatch.DrawString(Art.Font, $"{Health}", new Vector2(Position.X - image.Width / 15, Position.Y - image.Height / 10), Color.White);
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Adds the behaviour to the enemy
        /// </summary>
        /// <param name="behaviour">The behaviour the enemy has</param>
        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator());
        }

        /// <summary>
        /// Applies the behaviours of the enemy
        /// </summary>
        private void ApplyBehaviours()
        {
            // Iterate over all the behaviours
            for (int i = 0; i < behaviours.Count; i++)
            {
                // If there is no next behaviour, remove the behaviour preceeding it
                if (!behaviours[i].MoveNext())
                    behaviours.RemoveAt(i--);
            }
        }

        /// <summary>
        /// HAndles collisions between another enemy
        /// </summary>
        /// <param name="other">The other enemy</param>
        public void HandleCollision(Enemy other)
        {
            // Check to see if the enemy is inside the other enemy
            var d = Position - other.Position;
            Velocity += 10 * d / (d.LengthSquared() + 1);
        }
        
        /// <summary>
        /// Runs whenever the enemy instance was shot
        /// </summary>
        public void WasShot()
        {
            // Decrement health
            Health--;

            // Add points for a successful shot
            // Each large meteor hit earns 100
            // Each middle meteor earns 50
            // Each small earns 30
            PlayerStatus.AddPoints(PointValue / 5);

            // If the enemy is active and the health is at 0, kill the enemy
            if (Health <= 0 && timeUntilStart <= 0)
            {
                KillEnemy();
            }
        }

        /// <summary>
        /// Kills an enemy
        /// </summary>
        /// <param name="isNewLife">Checks to see whether or not the player died and the enemy needs to
        ///                         be killed without generating new subenemies. False by default</param>
        public void KillEnemy(bool isNewLife = false)
        {
            // Expire the entity
            IsExpired = true;
            
            // Check to see if the enemy was not killed by virtue of a new life
            if (!isNewLife)
            {
                // Add points and increase the multiplier
                PlayerStatus.AddPoints(PointValue);
                PlayerStatus.IncreaseMultiplier();

                // Create 2 random hues
                float hue1 = rand.NextFloat(0, 6);
                float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;

                // Extract the colours from the two hues
                Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
                Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);

                // Create a particle explosion of 120 particles
                for (int i = 0; i < 120; i++)
                {
                    // Set the particle speed
                    float speed = 18f * (1f - 1 / rand.NextFloat(1, 10));

                    // Create a new particle state
                    var state = new ParticleState()
                    {
                        Velocity = rand.NextVector2(speed, speed),
                        Type = ParticleType.Enemy,
                        LengthMultiplier = 1
                    };

                    // Lerp the colour between the two colours defined above
                    Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));

                    // Create the particle
                    GameBase.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
                }

                // Play an explosion sound
                Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

                // NOTE: I use different X velocities to add to the illusion of weight
                // If the enemy is a large meteor, spawn in two middle meteors
                if (Type == EnemyType.LargeMeteor)
                {
                    EntityManager.Add(MiddleMeteor(Position, new Vector2(5, -6)));
                    EntityManager.Add(MiddleMeteor(Position, new Vector2(-5, -6)));
                }
                // If the enemy is a middle meteor, spawn in two small meteors
                else if (Type == EnemyType.MiddleMeteor)
                {
                    EntityManager.Add(SmallMeteor(Position, new Vector2(7, -6)));
                    EntityManager.Add(SmallMeteor(Position, new Vector2(-7, -6)));
                    EntityManager.Add(SmallMeteor(Position, new Vector2(0, 6)));
                }
            }
        }
        
        /// <summary>
        /// Follows the player
        /// </summary>
        /// <param name="acceleration">The amount the player accelerates by</param>
        IEnumerable<int> FollowPlayer(float acceleration)
        {
            // Loop infinitely
            while (true)
            {
                // If the player is not dead, follow the player
                if (!PlayerShip.Instance.IsDead)
                    Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);

                // Change the orientation of the enemy
                if (Velocity != Vector2.Zero)
                    Orientation = Velocity.ToAngle();
                
                yield return 0;
            }
        }

        /// <summary>
        /// Moves the enemy randomly
        /// </summary>
        IEnumerable<int> MoveRandomly()
        {
            // Gets a random direction
            float direction = rand.NextFloat(0, MathHelper.TwoPi);

            // Loop infinitely
            while (true)
            {
                // Add a small, random value to the direction
                direction += rand.NextFloat(-0.1f, 0.1f);

                // Wrap the angle around the direction
                direction = MathHelper.WrapAngle(direction);
                
                for (int i = 0; i < 6; i++)
                {
                    // Alter the velocity based on the direction
                    Velocity += 0.4f * new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));

                    // Rotate the enemy
                    Orientation -= 0.05f;

                    // Inflate the bounds according to the image
                    var bounds = GameBase.Viewport.Bounds;
                    bounds.Inflate(-image.Width / 2 - 1, -image.Height / 2 - 1);

                    // If the enemy is outside the bounds, make it move away from the edge
                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (GameBase.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    
                    yield return 0;
                }
            }
        }

        /// <summary>
        /// Move the enemy according to gravity
        /// </summary>
        /// <param name="vel">Takes in a starting velocity</param>
        /// <returns></returns>
        IEnumerable<int> GravityMove(Vector2 vel)
        {
            // Loop infinitely
            while (true)
            {
                // Set the acceleration as g divided by the number of frames per second
                vel.Y += 9.81f/60;

                // Add it to the velocity property of the enemy
                Velocity = vel;
                
                // Check to see if the bottom wall was hit
                if (GameBase.ScreenSize.Y <= Position.Y + (image.Height / 2))
                {
                    // Move the enemy away to prevent clipping and reverse the velocity
                    vel.Y *= -1;
                    Position.Y -= (Velocity.Y + 1);
                }
                // Check to see if the side walls were hit
                if (GameBase.ScreenSize.X <= Position.X + (image.Width / 2) || 0 >= Position.X - (image.Width / 2))
                {
                    // Reverse direction
                    vel.X *= -1;

                    // Move the enemy away to prevent clipping
                    Position.X -= (0 >= Position.X - (image.Width / 2)) ? (Velocity.X - 1) : (Velocity.X + 1);
                }

                yield return 0;
            }
        }
    }
}
