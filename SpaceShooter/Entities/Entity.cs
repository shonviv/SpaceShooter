/*
 * Author: Shon Vivier
 * File Name: Entity.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/20/2019
 * Description: An object representing an entity
*/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    abstract class Entity
    {
        // The image associated with the entity
        protected Texture2D image;

        // The tint of the image. This will also allow changing the transparency
        protected Color color = Color.White;

        // The position and velocity of each entity
        public Vector2 Position;
        public Vector2 Velocity;

        // The orientation of the image
        public float Orientation;

        // Used for circular collision detection
        public float Radius = 20;  

        // True if the entity was destroyed and should be deleted
        public bool IsExpired;      

        // Returns the size of the iamge or zero if there is no image
        public Vector2 Size
        {
            get
            {
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        // Inhereted methods
        public abstract void Update();

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Draws the entity
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
        }
    }
}
