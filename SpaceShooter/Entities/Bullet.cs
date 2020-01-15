/*
 * Author: Shon Vivier
 * File Name: Bullet.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/20/2019
 * Description: Handles the bullet entity
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    class Bullet : Entity
    {
        // Create and define a new random
        private static Random rand = new Random();

        /// <summary>
        /// Creates the bullet entity
        /// </summary>
        /// <param name="position">Position of the bullet</param>
        /// <param name="velocity">Velocity of the bullet</param>
        public Bullet(Vector2 position, Vector2 velocity)
        {
            image = Art.Bullet;
            Position = position;
            Velocity = velocity;
            Orientation = Velocity.ToAngle();
            Radius = 8;
        }

        /// <summary>
        /// Handles the update loop
        /// </summary>
        public override void Update()
        {
            // Change the orientation of the bullets
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            Position += Velocity;

            // Delete bullets that go off-screen
            if (!GameBase.Viewport.Bounds.Contains(Position.ToPoint()))
            {
                // Expire the bullets
                IsExpired = true;

                // Create a particle explosion
                for (int i = 0; i < 30; i++)
                    GameBase.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                        new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });

            }
        }
    }
}
