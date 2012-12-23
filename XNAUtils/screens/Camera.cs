#region File Info
/*
 * Camera.cs
 */
#endregion
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

namespace XNAUtils.screens
{
    public class Camera
    {

        #region Fields

        /// <summary>
        /// Native supported x resolution
        /// </summary>
        float maxX = 1920.0f;

        /// <summary>
        /// Native supported y resolution
        /// </summary>
        float maxY = 1080.0f;

        /// <summary>
        /// Map width
        /// </summary>
        int mapWidth;

        /// <summary>
        /// Map Height
        /// </summary>
        int mapHeight;
        
        /// <summary>
        /// Resolution of the received screen.
        /// </summary>
        public Vector2 resolution;

        /// <summary>
        /// Position of the camera.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// Where the camera is centered relative to the screen.
        /// This should be the exact center of the given screen resolution.
        /// </summary>
        Vector2 originOffset;

        #endregion

        #region Properties

        /// <summary>
        /// Accesses the camera's map width
        /// </summary>
        public int MapWidth { get { return mapWidth; } set { mapWidth = value; } }

        /// <summary>
        /// Accesses the camera's map height.
        /// </summary>
        public int MapHeight { get { return mapHeight; } set { mapHeight = value; } }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the camera given the game.
        /// </summary>
        /// <param name="game">The game object</param>
        public Camera(UtilGame game)
        {
            resolution = new Vector2(game.Configuration.screenWidth, game.Configuration.screenHeight);
            position = Vector2.Zero;
            originOffset = new Vector2(maxX / 2.0f, maxY / 2.0f);
            mapWidth = 2000;
            mapHeight = 2000;
        }

        /// <summary>
        /// Initializes the camera with a given max screen size.
        /// </summary>
        /// <param name="game">The game object</param>
        /// <param name="maxWidth">Maximum screen width</param>
        /// <param name="maxHeight">Maximum screen height</param>
        public Camera(UtilGame game, int maxWidth, int maxHeight)
        {
            resolution = new Vector2(game.Configuration.screenWidth, game.Configuration.screenHeight);
            position = Vector2.Zero;
            originOffset = new Vector2(maxX / 2.0f, maxY / 2.0f);

            mapWidth = maxWidth;
            mapHeight = maxHeight;
        }

        #endregion

        #region Modification

        /// <summary>
        /// Moves the camera to the given position.
        /// </summary>
        /// <param name="position">The position vector</param>
        public void Move(Vector2 position)
        {
            this.position = position;
            this.position.X = MathHelper.Clamp(position.X, originOffset.X, mapWidth - originOffset.X);
            this.position.Y = MathHelper.Clamp(position.Y, originOffset.Y, mapHeight - originOffset.Y);
        }

        #endregion

        #region Transformations

        /// <summary>
        /// Returns the screen transformation for the main game.
        /// </summary>
        /// <returns>Screen transformation matrixs</returns>
        public Matrix ScreenTransformation()
        {
            Vector3 translation = new Vector3(position - originOffset, 0);

            return Matrix.CreateTranslation(-translation) *
                   Matrix.CreateScale(resolution.X / maxX, resolution.Y / maxY, 1.0f);
        }

        /// <summary>
        /// Returns the screen transformation for the UI
        /// </summary>
        /// <returns></returns>
        public Matrix UITransformation()
        {
            return Matrix.CreateScale(resolution.X / maxX, resolution.Y / maxY, 1.0f);
        }

        /// <summary>
        /// Returns the coordinates of a point on the screen in world coordinates.
        /// </summary>
        /// <param name="screenPosition">Position on the screen</param>
        public Vector2 screenToWorldUI(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(UITransformation()));
        }

        #endregion

        #region Info

        /// <summary>
        /// Gets a rectangle indicating the bounds of the current view.
        /// </summary>
        /// <returns>Rectangle representing current view</returns>
        public Rectangle visibleArea()
        {
            return new Rectangle((int) (position.X - originOffset.X), (int) (position.Y - originOffset.Y), (int) maxX, (int) maxY);
        }

        #endregion
    }
}
