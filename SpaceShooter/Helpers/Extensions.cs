/*
 * Author: Shon Vivier
 * File Name: Extensions.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/18/2019
 * Description: A class of extensions methods that aid with mathematical calculations
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
    static class Extensions
    {
        /// <summary>
        /// Draws a line between two points given a starting and end vector
        /// </summary>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 2f)
        {
            Vector2 delta = end - start;
            spriteBatch.Draw(Art.Pixel, start, null, color, delta.ToAngle(), new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Converts a vector to an angle
        /// </summary>
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        /// <summary>
        /// Scales a vector to a length
        /// </summary>
        public static Vector2 ScaleTo(this Vector2 vector, float length)
        {
            return vector * (length / vector.Length());
        }


        /// <summary>
        /// Converts a vector to a point
        /// </summary>
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
        
        /// <summary>
        /// Randomly generates a float value
        /// </summary>
        public static float NextFloat(this Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        
        /// <summary>
        /// Randomly generates a Vector2 value
        /// </summary>
        public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
        {
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = rand.NextFloat(minLength, maxLength);
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }
    }
}
