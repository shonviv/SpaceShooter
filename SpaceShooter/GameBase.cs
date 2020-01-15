/*
 * Author: Shon Vivier
 * File Name: GameBase.cs
 * Project Name: SpaceShooter
 * Creation Date: 5/17/2019
 * Modified Date: 5/20/2019
 * Description: The GameBase handles all operations regarding scene management, updating, and drawing
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceShooter.Screens;

namespace ShapeBlaster
{
    public class GameBase : Game
    {
        // Property that keeps track of the current instance
        public static GameBase Instance { get; private set; }

        // Viewport used in determining screen sizes
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }

        // A property that returns the screen dimensions as a Vector2
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }

        // An instance of GameTime that helps the Player keep track of their score multiplier time
        public static GameTime GameTime { get; private set; }

        // Particle Manager
        public static ParticleManager<ParticleState> ParticleManager { get; private set; }
        
        // Declare SpriteBatch and Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Declare gameplay screen
        GameplayScreen gameplayScreen;

        // Define the initial game state as Main Menu
        public static GameState State = GameState.MainMenu;

        // Enum responsible for mantaining all the different possible game states
        public enum GameState
        {
            MainMenu,
            Instructions,
            ModeSelection,
            ClassicGameplay,
            FreeGameplay,
        }

        /// <summary>
        /// Constructor for the game root
        /// </summary>
        public GameBase()
        {
            // Set the instance property as the class instance and define graphics
            Instance = this;
            graphics = new GraphicsDeviceManager(this);

            // Change the content directory to search in /Content/ first
            Content.RootDirectory = "Content";

            // Set the preferred screen sizes
            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 600;

            // Define the particle manager and set the maximum particle capacity
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
        }
        
        /// <summary>
        /// Responsible for loading all content
        /// </summary>
        protected override void LoadContent()
        {
            // Define the spritebatch and load all the art and sound content
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            Sound.Load(Content);
        }

        /// <summary>
        /// The base update loop calls update loops corresponding to the current game state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Update(GameTime gameTime)
        {
            // Update the game time
            GameTime = gameTime;

            // Switch over the current state
            switch (State)
            {
                // Call the corresponding update functions for each menu
                case GameState.MainMenu:
                    MainMenuScreen.Update();
                    break;
                case GameState.Instructions:
                    InstructionsScreen.Update();
                    break;
                case GameState.ModeSelection:
                    ModeSelectionScreen.Update();
                    break;

                // Game Modes
                case GameState.ClassicGameplay:
                    // Create a new gameplay screen if it is null
                    if (gameplayScreen == null)
                    {
                        gameplayScreen = new GameplayScreen();
                    }

                    // Update the gameplay screen
                    gameplayScreen.Update();
                    break;

                // The gameplay screens are identical and differ only in the EnemyManager (what enemies spawn)
                // and PlayerShip (how the player is controlled), meaning GameplayScreen can be used generically
                // in handling all operations regarding drawing and updating gameplay
                case GameState.FreeGameplay:
                    if (gameplayScreen == null)
                    {
                        gameplayScreen = new GameplayScreen();
                    }

                    gameplayScreen.Update();
                    break;
            }

            // Check to see if the game state is not in gameplay
            if (State != GameState.ClassicGameplay && State != GameState.FreeGameplay)
            {
                // Set the gameplay screen to null since the player ship controls and enemies are agnostic
                // to the current scene. The gameplay screen prevents updates to the player and enemies but
                // will still draw them unless the gameplay screen is reset entirely 
                gameplayScreen = null;
            }
            
            // Update the root update loop
            base.Update(gameTime);
        }

        /// <summary>
        /// The base draw loop handles drawing all draw loops corresponding to the current game state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Switch based on the current scene
            switch (State)
            {
                // Call each corresponding draw method
                case GameState.MainMenu:
                    MainMenuScreen.Draw(spriteBatch);
                    break;
                case GameState.Instructions:
                    InstructionsScreen.Draw(spriteBatch);
                    break;
                case GameState.ModeSelection:
                    ModeSelectionScreen.Draw(spriteBatch);
                    break;

                // If the gameplayScreen is not defined because the state updates asynchronously from
                // the update loop creating a new instance of gameplayScreen, ignore the current frame
                // and wait until the gameplayScreen is defined
                case GameState.ClassicGameplay:
                    if (gameplayScreen != null)
                        gameplayScreen.Draw(spriteBatch);
                    break;
                case GameState.FreeGameplay:
                    if (gameplayScreen != null)
                        gameplayScreen.Draw(spriteBatch);
                    break;
            }
        }
    }
}
