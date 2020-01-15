/*
 * Author: Shon Vivier
 * File Name: Art.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/19/2019
 * Description: Handles loading all art content
*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    static class Art
    {
        // Entity image properties
        public static Texture2D Player { get; private set; }

        public static Texture2D Seeker { get; private set; }

        public static Texture2D Wanderer { get; private set; }

        public static Texture2D Bullet { get; private set; }

        public static Texture2D Pointer { get; private set; }

        public static Texture2D LargeMeteor { get; private set; }

        public static Texture2D MiddleMeteor { get; private set; }

        public static Texture2D SmallMeteor { get; private set; }

        // Extension image properties
        public static Texture2D LineParticle { get; private set; }

        public static Texture2D Glow { get; private set; }

        public static Texture2D Pixel { get; private set; }

        // Default font properties
        public static SpriteFont Font { get; private set; }

        /// <summary>
        /// Loads all of the art content
        /// </summary>
        /// <param name="content">Takes in a base content manager to update</param>
        public static void Load(ContentManager content)
        {
            // Loads all the entity textures
            Player = content.Load<Texture2D>("Art/Player");
            Seeker = content.Load<Texture2D>("Art/Seeker");
            Wanderer = content.Load<Texture2D>("Art/Wanderer");
            Bullet = content.Load<Texture2D>("Art/Bullet");
            LargeMeteor = content.Load<Texture2D>("Art/LargeMeteor");
            MiddleMeteor = content.Load<Texture2D>("Art/MiddleMeteor");
            SmallMeteor = content.Load<Texture2D>("Art/SmallMeteor");

            // Loads the mouse cursor
            Pointer = content.Load<Texture2D>("Art/Pointer");

            // Loads the texture used for a line particle and glow
            LineParticle = content.Load<Texture2D>("Art/Laser");
            Glow = content.Load<Texture2D>("Art/Glow");

            // Creates a single white pixel used in drawing lines
            Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            // Loads the font
            Font = content.Load<SpriteFont>("Font");
        }
    }
}
