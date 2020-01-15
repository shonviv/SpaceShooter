/*
 * Author: Shon Vivier
 * File Name: PlayerShip.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/18/2019
 * Modified Date: 5/20/2019
 * Description: Handles all operations regarding the ship controller
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
    class PlayerShip : Entity
    {
        // Declare an instance of the playership as a field
        private static PlayerShip instance;

        // Create an instance property returning a new playership
        public static PlayerShip Instance
        {
            get
            {
                // If the instance field is null, create a new playership
                if (instance == null)
                    instance = new PlayerShip();

                // Return the instance field
                return instance;
            }
        }

        // Defines the amount of frames to wait before firing a new bullet
        const int cooldownFrames = 9;

        // Defines how many cooldown frames remain
        int cooldowmRemaining = 0;

        // Defines how many cooldown frames exist before respawning the ship
        int framesUntilRespawn = 0;

        // Say the ship is dead if there are frames that need to pass before the ship respawns
        public bool IsDead { get { return framesUntilRespawn > 0; } }

        // Create a new random
        static Random rand = new Random();

        /// <summary>
        /// Constructor that initializes the player ship
        /// </summary>
        private PlayerShip()
        {
            // Load the image and position information
            image = Art.Player;
            Position = new Vector2(GameBase.ScreenSize.X / 2, GameBase.ScreenSize.Y - 10);
            Radius = 10;
        }

        /// <summary>
        /// The update loop for the player ship
        /// </summary>
        public override void Update()
        {
            // Check to see if the ship is dead
            if (IsDead)
            {
                // Wait until the ship can respawn
                if (--framesUntilRespawn == 0)
                {
                    // Check to see if the player has no more lives
                    if (PlayerStatus.Lives <= 0)
                    {
                        // Reset all game information and go back to the main menu
                        EntityManager.Reset();
                        PlayerStatus.Reset();
                        GameBase.State = GameBase.GameState.MainMenu;
                    }
                }

                // End the update loop
                return;
            }

            // NOTE: With regards to the firing, the maximum number of alloted shots on the screen is less than 10.
            // In this kind of game, I personally find it incredibly tedious to have to worry about when to fire and
            // a maximum carrying capacity of shots. Though it could be implemented easily, I prefer having an automatic
            // stream of bullets in this kind of game

            // Set the bullet aim direction as going directly up for the classic game mode
            Vector2 aim = new Vector2(0, -1);
            
            // Check to see if the player is in the free game mode
            if (GameBase.State == GameBase.GameState.FreeGameplay)
            {
                // Set the aim as the mouse direction
                aim = HandleInput.GetMouseAimDirection();
            }

            // If there is no more cooldown and there is a position to aim in
            if (aim.LengthSquared() > 0 && cooldowmRemaining <= 0)
            {
                // Reset the cooldown
                cooldowmRemaining = cooldownFrames;

                // Get the aim angle
                float aimAngle = aim.ToAngle();
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
                
                // Define the bullet velocity
                Vector2 vel = 8f * new Vector2((float)Math.Cos(aimAngle), (float)Math.Sin(aimAngle));
                
                // Define the bullet offset
                Vector2 offset = Vector2.Transform(new Vector2(40, -8), aimQuat);

                // Add the bullet to the entity manager
                EntityManager.Add(new Bullet(Position + offset, vel));

                // Player a shot sound effect
                Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
            }

            // If there is a cooldown remaining, continue to wait
            if (cooldowmRemaining > 0)
                cooldowmRemaining--;

            // Set the speed in pixels for the bullet
            const float speed = 8;

            // Set the velocity of the bullet as the speed times the movement direction
            Velocity += speed * HandleInput.GetMovementDirection();

            // Increase the position by the velocity and clamp it to the game window
            Position += Velocity;
            Position = Vector2.Clamp(Position, Size / 2, GameBase.ScreenSize - Size / 2);

            // Set the bullet orientation
            if (Velocity.LengthSquared() > 0)
                Orientation = Velocity.ToAngle();

            // Generate the ship exhaust fire upon movement and reset the velocity
            MakeExhaustFire();
            Velocity = Vector2.Zero;
        }

        /// <summary>
        /// Handles making a small particle effect for the ship's exhaust upon movement
        /// </summary>
        private void MakeExhaustFire()
        {
            // Check to see if the ship is moving
            if (Velocity.LengthSquared() > 0.1f)
            {
                // Set up the orientation and rotation values
                Orientation = Velocity.ToAngle();
                Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);

                // Get the time value as the game time in seconds
                double t = GameBase.GameTime.TotalGameTime.TotalSeconds;

                // The primary velocity of the particles is 3 pixels per frame in the direction opposite to which the ship is travelling
                Vector2 baseVel = Velocity.ScaleTo(-3);

                // Calculate the sideways velocity for the two side streams. The direction is perpendicular to the ship's velocity and the
                // magnitude varies sinusoidally
                Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));

                // Create a deep red side colour and an orange-yellow mid colour
                Color sideColor = new Color(200, 38, 9);   
                Color midColor = new Color(255, 187, 30);

                // Store the position of the ship's exhaust pipe
                Vector2 pos = Position + Vector2.Transform(new Vector2(-25, 0), rot);

                // Add some transparency
                const float alpha = 0.7f;

                // Create the ,iddle particle stream
                Vector2 velMid = baseVel + rand.NextVector2(0, 1);
                GameBase.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(velMid, ParticleType.Enemy));
                GameBase.ParticleManager.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(velMid, ParticleType.Enemy));

                // Create the side particle streams
                Vector2 vel1 = baseVel + perpVel + rand.NextVector2(0, 0.3f);
                Vector2 vel2 = baseVel - perpVel + rand.NextVector2(0, 0.3f);

                GameBase.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(vel1, ParticleType.Enemy));
                GameBase.ParticleManager.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(vel2, ParticleType.Enemy));

                GameBase.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(vel1, ParticleType.Enemy));
                GameBase.ParticleManager.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1), new ParticleState(vel2, ParticleType.Enemy));
            }
        }

        /// <summary>
        /// Draws the player ship and exhaust fire
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Only draw if the ship is not dead
            if (!IsDead)
                base.Draw(spriteBatch);
        }

        /// <summary>
        /// Handles death for the ship
        /// </summary>
        public void Kill()
        {
            // Remove a life
            PlayerStatus.RemoveLife();

            // Reset the position
            Position = new Vector2(GameBase.ScreenSize.X / 2, GameBase.ScreenSize.Y - 10);

            // Set the frames until respawn as 120 if the game is not over, and 300 if the game is over
            framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;

            // Create a yellow explosion colour
            Color explosionColor = new Color(0.8f, 0.8f, 0.4f);

            // Create death particles
            for (int i = 0; i < 1200; i++)
            {
                // Define the particle speed and colour
                float speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
                Color color = Color.Lerp(Color.White, explosionColor, rand.NextFloat(0, 1));

                // Create a new particle state
                var state = new ParticleState()
                {
                    Velocity = rand.NextVector2(speed, speed),
                    Type = ParticleType.None,
                    LengthMultiplier = 1
                };

                // Create the individual particle
                GameBase.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
            }
        }
    }
}
