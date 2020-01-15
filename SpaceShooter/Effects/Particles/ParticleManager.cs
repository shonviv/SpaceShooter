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
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace ShapeBlaster
{
    public class ParticleManager<T>
    {
        // This delegate will be called for each particle
        private Action<Particle> updateParticle;
        private CircularParticleArray particleList;

        /// <summary>
        /// Allows creation of particles
        /// </summary>
        /// <param name="capacity">The maximum number of particles. An array of this size will be pre-allocated</param>
        /// <param name="updateParticle">A delegate that lets you specify custom behaviour for your particles. Called once per particle, per frame</param>
        public ParticleManager(int capacity, Action<Particle> updateParticle)
        {
            // Update the particle list
            this.updateParticle = updateParticle;
            particleList = new CircularParticleArray(capacity);

            // Populate the list with empty particle objects, for reuse
            for (int i = 0; i < capacity; i++)
                particleList[i] = new Particle();
        }

        /// <summary>
        /// Update particle state every frame
        /// </summary>
        public void Update()
        {
            // Update each particle
            int removalCount = 0;
            for (int i = 0; i < particleList.Count; i++)
            {
                // Get the particle from the particle list
                var particle = particleList[i];

                // Update the particle
                updateParticle(particle);
                
                // Decrement the percent life of the particle as it approaches the max duration
                particle.PercentLife -= 1f / particle.Duration;

                // Sift deleted particles to the end of the list
                Swap(particleList, i - removalCount, i);

                // If the alpha < 0, delete this particle
                if (particle.PercentLife < 0)
                    removalCount++;
            }

            // Remove the amount of particles to be removed from the particle list
            particleList.Count -= removalCount;
        }

        /// <summary>
        /// Swaps 2 particles in the particle list
        /// </summary>
        /// <param name="list">The particle list</param>
        /// <param name="index1">The index of the first particle</param>
        /// <param name="index2">The index of the second particle</param>
        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            // Swap the particles using a temporary variable
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// Draw the particles
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Iterate over all the particles
            for (int i = 0; i < particleList.Count; i++)
            {
                // Select the current particle
                var particle = particleList[i];

                // Gets the particle point of origin and draws it around the origin
                Vector2 origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);
                spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation, origin, particle.Scale, 0, 0);
            }
        }

        /// <summary>
        /// Overload method that calls another overload after simplifying parameters
        /// </summary>
        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
        {
            CreateParticle(texture, position, tint, 150, new Vector2(5), state, theta);
        }

        /// <summary>
        /// Creates a particle
        /// </summary>
        public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
        {
            // Create new instance of a particle
            Particle particle;
            
            // If the list is full, overwrite the oldest particle and rotate the circular list
            if (particleList.Count == particleList.Capacity)
            {
                particle = particleList[0];
                particleList.Start++;
            }
            else
            {
                particle = particleList[particleList.Count];
                particleList.Count++;
            }

            // Create the particle and set the properties
            particle.Texture = texture;
            particle.Position = position;
            particle.Tint = tint;

            particle.Duration = duration;
            particle.PercentLife = .4f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;
        }

        /// <summary>
        /// Destroys all particles
        /// </summary>
        public void Clear()
        {
            particleList.Count = 0;
        }

        // Returns the particle count
        public int ParticleCount
        {
            get { return particleList.Count; }
        }

        // Helper class the has all particle information
        public class Particle
        {
            // Declare the particle properties
            public Texture2D Texture;
            public Vector2 Position;
            public float Orientation;

            public Vector2 Scale = Vector2.One;

            public Color Tint;
            public float Duration;
            public float PercentLife = 1f;
            public T State;
        }

        // Represents a circular array with an arbitrary starting point. It's useful for efficiently overwriting
        // the oldest particles when the array gets full. Simply overwrite particleList[0] and advance Start
        private class CircularParticleArray
        {
            // The starting index
            private int start;

            // Returns the start
            public int Start
            {
                get { return start; }
                set { start = value % list.Length; }
            }

            // The count of particles and capacity
            public int Count { get; set; }

            public int Capacity { get { return list.Length; } }

            // List of particles
            private Particle[] list;

            // For serialization
            public CircularParticleArray() { }  

            // Creates a new list of particles of a certain capacity
            public CircularParticleArray(int capacity)
            {
                list = new Particle[capacity];
            }

            // Creates the circular array (wrapping)
            public Particle this[int i]
            {
                get { return list[(start + i) % list.Length]; }
                set { list[(start + i) % list.Length] = value; }
            }
        }
    }
}
