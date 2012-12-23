using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNAUtils.screens.management;
using XNAUtils.support;
using XNAUtils.support.particles;

namespace XNAUtils.screens
{
    public class GameplayScreen : GameScreen
    {
        // Member variables for the screen.
        #region Fields

        /// <summary>
        /// Camera object for view transformations
        /// </summary>
        Camera camera;

        /// <summary>
        /// Indicates if the game is paused.
        /// </summary>
        public bool paused = false;

        /// <summary>
        /// Cursor world position
        /// </summary>
        private Vector2 cursorWorldPos;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for the main gameplay screen
        /// </summary>
        /// <param name="camera">Camera to draw elements correctly</param>
        public GameplayScreen(Camera camera)
        {
            this.camera = camera;
        }

        #endregion

        #region Loading

        /// <summary>
        /// Load content for the gameplay screen.
        /// </summary>
        public override void LoadContent()
        {
            // Get content manager from the game object.
            ContentManager content = ScreenManager.Game.Content;
            base.LoadContent();
        }

        #endregion

        #region Update

        /// <summary>
        /// Update the gameplay screen.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // A good way to test if your screens are transitioning correctly is to have them just draw a different color.
            //ScreenManager.GraphicsDevice.Clear(Color.Blue);
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

            SpriteBatch sb = ScreenManager.SpriteBatch;

            // There are various draw methods. The one belows draws the sprites with a matrix transform.
            // sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
            //         null, null, null, null, camera.UITransformation());
            //
            // The different default camerea transformations are UI (Scale relative to absolute screen position) and World (move a camera around a larger area)
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.UITransformation());

            // Draw stuff here

            sb.End();

            // Fade this guy in if coming in or going out
            ScreenManager.FadeBackBufferToBlack(1.0f - TransitionAlpha);

            base.Draw(gameTime);
        }

        #endregion

        /// <summary>
        /// Handles input for the screen.
        /// </summary>
        /// <param name="input">InputState manager object.</param>
        public override void HandleInput(InputState input)
        {
            // If escape is pressed, pause the game
            if (input.IsPauseGame(null))
            {
                paused = true;
            }
            
            Vector2 screenPos = input.MousePosition;
            cursorWorldPos = camera.screenToWorldUI(screenPos);

            base.HandleInput(input);
        }
    }
}
