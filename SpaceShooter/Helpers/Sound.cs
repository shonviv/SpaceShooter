/*
 * Author: Shon Vivier
 * File Name: Sound.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/19/2019
 * Description: Handles loading all sound content
*/

using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ShapeBlaster
{
    static class Sound
    {
        // Property for the current song playing
        public static Song Music { get; private set; }

        // Create new random
        private static readonly Random rand = new Random();

        // Create an array storing all the different explosion sound effects
        private static SoundEffect[] explosions;

        // Return a random explosion sound
        public static SoundEffect Explosion { get { return explosions[rand.Next(explosions.Length)]; } }

        // Create an array storing all the different shot sound effects
        private static SoundEffect[] shots;

        // Return a random shot sound
        public static SoundEffect Shot { get { return shots[rand.Next(shots.Length)]; } }

        // Create an array storing all the different spawn sound effects
        private static SoundEffect[] spawns;

        // Return a random spawn sound
        public static SoundEffect Spawn { get { return spawns[rand.Next(spawns.Length)]; } }

        /// <summary>
        /// Loads all of the sound content
        /// </summary>
        /// <param name="content">Takes in a base content manager to update</param>
        public static void Load(ContentManager content)
        {
            // Set the music
            Music = content.Load<Song>("Sound/Music");

            // Load all sounds of each category into an array
            explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Sound/explosion-0" + x)).ToArray();
            shots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Sound/shoot-0" + x)).ToArray();
            spawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Sound/spawn-0" + x)).ToArray();
        }
    }
}
