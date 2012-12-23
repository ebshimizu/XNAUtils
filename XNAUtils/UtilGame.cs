using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNAUtils.screens;
using XNAUtils.screens.management;
using XNAUtils.support;

namespace XNAUtils
{
    /// <summary>
    /// This is the main type for your game
    /// This game modifies the default generated game class to set it up for use with 
    /// the ScreenManager system found in the XNAUtils namespace
    /// </summary>
    public class UtilGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Graphics manager option for setting resolution and other related settings
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// Shared sprite batch drawing object.
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Object that manages the screens involved in the game
        /// </summary>
        ScreenManager screenManager;

        /// <summary>
        /// Game configuration object.
        /// </summary>
        Configuration configuration;

        /// <summary>
        /// Camera object.
        /// </summary>
        Camera camera;

        public Configuration Configuration { get { return configuration; } set { configuration = value; } }

        public UtilGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };

            // Initialize configuration object
            configuration = new Configuration();

            // Initialize camera
            camera = new Camera(this);

            // Initialize the screen manager
            screenManager = new ScreenManager(this, camera, configuration, graphics);

            // Add the screen manager to the components list.
            // Components are special objects that respond to the draw and update loops
            // at a different time than the main application. The main implication of this
            // when using the screen manager is that you don't actually call draw or update
            // on the manager explicitly in this game loop.
            Components.Add(screenManager);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Apply the hard-coded configuration settings. You can load configuration values
            // from a text file too, but you'd have to write that.
            configuration.Apply(this, graphics);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Here you would add the first screen you want to see in the game.
            // By default, we just dump a game screen on there able to be controlled by any player:
            screenManager.AddScreen(new GameplayScreen(camera), null);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // IMPORTANT: Do not add your update logic here. Add it in the screens.
            // If you have global update logic you want to happen (like the back button exiting the game)
            // place it here.

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // IMPORTANT: Do not add your draw code here. Add it in your screens.
            // If you have global draw code you want to have happen (clearing to a color) do it here.

            base.Draw(gameTime);
        }
    }
}
