/*
 * Author: Shon Vivier
 * File Name: ParticleState.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/19/2019
 * Modified Date: 5/20/2019
 * Description: Used for creating and defining the states and types of particles
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    // Defines the different types of particles
    public enum ParticleType
    {
        None,
        Enemy,
        Bullet,
        IgnoreGravity
    }

    public struct ParticleState
    {
        // Declare the velocity, type, and length multiplier of the particle
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;

        // Create a new random
        private static Random rand = new Random();

        /// <summary>
        /// Creates a particle state
        /// </summary>
        /// <param name="velocity">Velocity of the particle</param>
        /// <param name="type">Type of the particle</param>
        /// <param name="lengthMultiplier">Length multiplier of the particle</param>
        public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
        {
            // Set the velocity, type, and length multiplier of the particle
            Velocity = velocity;
            Type = type;
            LengthMultiplier = lengthMultiplier;
        }

        /// <summary>
        /// Gets a random particle state between 2 velocities
        /// </summary>
        /// <param name="minVel">The minimum velocity</param>
        /// <param name="maxVel">The maximum velocity</param>
        /// <returns></returns>
        public static ParticleState GetRandom(float minVel, float maxVel)
        {
            // Create the state and set random properties
            var state = new ParticleState();
            state.Velocity = rand.NextVector2(minVel, maxVel);
            state.Type = ParticleType.None;
            state.LengthMultiplier = 1;

            return state;
        }

        /// <summary>
        /// Updates the particle with a state
        /// </summary>
        /// <param name="particle">The particle to be updated</param>
        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
        {
            // Retrieve the speed and velocity of the particle
            var vel = particle.State.Velocity;
            float speed = vel.Length();

            // Using Vector2.Add() should be slightly faster than doing "x.Position += vel;" because the Vector2s
            // are passed by reference and don't need to be copied. Since we may have to update a very large 
            // number of particles, this method is a good candidate for optimizations
            Vector2.Add(ref particle.Position, ref vel, out particle.Position);

            // Fade the particle if its PercentLife or speed is low
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;

            // Set the particle tint as the transparency
            particle.Tint.A = (byte)(255 * alpha);
            
            // The length of bullet particles will be less dependent on their speed than other particles
            if (particle.State.Type == ParticleType.Bullet)
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
            else
                particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

            // Set the orientation of the particle
            particle.Orientation = vel.ToAngle();

            // Set the particle position and dimensions
            var pos = particle.Position;
            int width = (int)GameBase.ScreenSize.X;
            int height = (int)GameBase.ScreenSize.Y;

            // Collide with the edges of the screen
            if (pos.X < 0)
                vel.X = Math.Abs(vel.X);
            else if (pos.X > width)
                vel.X = -Math.Abs(vel.X);
            if (pos.Y < 0)
                vel.Y = Math.Abs(vel.Y);
            else if (pos.Y > height)
                vel.Y = -Math.Abs(vel.Y);

            // Denormalized floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;
            else if (particle.State.Type == ParticleType.Enemy)
                vel *= 0.94f;
            else
                // rand.Next() isn't thread-safe, so use the position for pseudo-randomness
                vel *= 0.96f + Math.Abs(pos.X) % 0.04f; 

            // Set the particle state velocity as the new velocity
            particle.State.Velocity = vel;
        }
    }
}
