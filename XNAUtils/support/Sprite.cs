using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAUtils.support
{
    public class Sprite
    {
        #region Fields
        /// <summary>
        /// How many frames of animation are in the texture
        /// </summary>
        int framecount;

        /// <summary>
        /// The current frame
        /// </summary>
        public int Frame;

        /// <summary>
        /// Number of frames in each row of the sprite sheet
        /// </summary>
        int framesPerRow;

        /// <summary>
        /// Number of rows in the sprite sheet
        /// </summary>
        int rows;

        /// <summary>
        /// Rotation factor
        /// </summary>
        float rotation;

        /// <summary>
        /// Sprite scale factor
        /// </summary>
        public float Scale;

        /// <summary>
        /// Draw depth. Only useful if the spritebatch draw mode is set to use it.
        /// </summary>
        float Depth;

        /// <summary>
        /// Sprite draw origin.
        /// </summary>
        Vector2 origin;

        /// <summary>
        /// The image containing all of the frames ordered in a row or a grid.
        /// </summary>
        Texture2D spriteSheet;

        Vector2 position;

        /// <summary>
        /// How much time each frame is played for.
        /// </summary>
        private float TimePerFrame;

        /// <summary>
        /// Total elapsed time.
        /// </summary>
        private float TotalElapsed;

        /// <summary>
        /// Indicates if playback for the texture is paused.
        /// </summary>
        private bool paused;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the pause state of the sprite
        /// </summary>
        public bool Paused
        {
            get { return paused; }
        }

        /// <summary>
        /// Returns the sprite's internal position. Not recommended to use.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The texture object
        /// </summary>
        public Texture2D SpriteSheet
        {
            get { return spriteSheet; }
            set { spriteSheet = value; }
        }

        /// <summary>
        /// Bounds of one frame of the sprite.
        /// </summary>
        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Width, Height); }
        }

        /// <summary>
        /// Width of a single sprite frame
        /// </summary>
        public int Width { get { return spriteSheet.Width / framesPerRow; } }

        /// <summary>
        /// Height of a single sprite frame
        /// </summary>
        public int Height { get { return spriteSheet.Height / rows; } }

        /// <summary>
        /// Accesses the origin variable.
        /// </summary>
        public Vector2 Origin { get { return origin; } set { origin = value; } }

        /// <summary>
        /// Rotation in radians.
        /// </summary>
        public float Rotation { get { return rotation; } set { rotation = value; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Simplified constructor for single-line animated sprite sheets.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCount"></param>
        /// <param name="fps"></param>
        public Sprite(Texture2D texture, int frameCount, int fps)
        {
            framecount = frameCount;
            this.framesPerRow = frameCount;
            rows = 1;
            spriteSheet = texture;
            TimePerFrame = (float)1 / fps;
            Frame = 0;
            TotalElapsed = 0;
            paused = false;
            Scale = 1.0f;
            rotation = 0.0f;
            Depth = 1.0f;
            origin = Vector2.Zero;
        }

        /// <summary>
        /// Basic constructor for an animated sprite for a 1-row sprite sheet.
        /// </summary>
        /// <param name="loadedTexture">The spritesheet to animate</param>
        /// <param name="frameCount">Number of frames in the animation</param>
        /// <param name="framesPerSecond">Frame rate for the animation</param>
        /// <param name="origin">Sprite origin</param>
        /// <param name="rotation">Sprite rotation</param>
        /// <param name="scale">Sprite scale</param>
        /// <param name="depth">Sprite draw depth</param>
        public Sprite(Texture2D loadedTexture, int frameCount, int framesPerSecond,
            Vector2 origin, float rotation,
            float scale, float depth)
        {
            this.origin = origin;
            this.rotation = rotation;
            this.Scale = scale;
            this.Depth = depth;
            framecount = frameCount;
            framesPerRow = frameCount;
            rows = framecount / framesPerRow;
            spriteSheet = loadedTexture;
            TimePerFrame = (float)1 / framesPerSecond;
            Frame = 0;
            TotalElapsed = 0;
            paused = false;
        }

        /// <summary>
        /// Constructor for a multi-line spritesheet animation.
        /// Each line in the spritesheet is required to have the same number of frames.
        /// </summary>
        /// <param name="loadedTexture">The spritesheet to animate</param>
        /// <param name="frameCount">Number of frames in the animation</param>
        /// <param name="framesPerSecond">Frame rate</param>
        /// <param name="origin">Sprite origin</param>
        /// <param name="rotation">Sprite rotation</param>
        /// <param name="scale">Sprite scale</param>
        /// <param name="depth">Sprite draw depth</param>
        /// <param name="framesRow">Number of frames per row</param>
        public Sprite(Texture2D loadedTexture, int frameCount, int framesPerSecond,
            Vector2 origin, float rotation,
            float scale, float depth, int framesRow)
        {
            this.origin = origin;
            this.rotation = rotation;
            this.Scale = scale;
            this.Depth = depth;
            framecount = frameCount;
            framesPerRow = framesRow;
            rows = framecount / framesPerRow;
            spriteSheet = loadedTexture;
            TimePerFrame = (float)1 / framesPerSecond;
            Frame = 0;
            TotalElapsed = 0;
            paused = false;
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the sprite with default color.
        /// </summary>
        /// <param name="spriteBatch">Game's spritebatch object to draw with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            DrawFrame(spriteBatch, Frame, position, Color.White, SpriteEffects.None);
        }

        /// <summary>
        /// Draws the sprite with an arbitrary color.
        /// </summary>
        /// <param name="spriteBatch">Game's spriteBatch object to draw with</param>
        /// <param name="color">The color to draw the sprite.</param>
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            DrawFrame(spriteBatch, Frame, position, color, SpriteEffects.None);
        }

        /// <summary>
        /// Draws the current frame given a spritebatch and a position.
        /// Calls the helper function to draw the current frame.
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing</param>
        /// <param name="screenPos">Pixel position of draw origin</param>
        public void Draw(SpriteBatch batch, Vector2 screenPos)
        {
            DrawFrame(batch, Frame, screenPos, Color.White, SpriteEffects.None);
        }

        /// <summary>
        /// Draws the current frame given a spritebatch, position, and color.
        /// Calls the helper function to draw the current frame.
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing</param>
        /// <param name="screenPos">Pixel position of draw origin</param>
        /// <param name="color">Color to draw</param>
        public void Draw(SpriteBatch batch, Vector2 screenPos, Color color)
        {
            DrawFrame(batch, Frame, screenPos, color, SpriteEffects.None);
        }

        /// <summary>
        /// Draws the current frame given a position and a sprite effect
        /// </summary>
        /// <param name="batch">SpriteBatch object</param>
        /// <param name="screenPos">Position vector</param>
        /// <param name="effect">Sprit effect to apply</param>
        public void Draw(SpriteBatch batch, Vector2 screenPos, SpriteEffects effect)
        {
            DrawFrame(batch, Frame, screenPos, Color.White, effect);
        }

        /// <summary>
        /// Draws a specific frame of the animation.
        /// Can be used to draw a static frame from a spritesheet.
        /// </summary>
        /// <param name="batch">SpriteBatch for drawing</param>
        /// <param name="frame">The frame to draw</param>
        /// <param name="screenPos">Pixel positon of draw origin</param>
        /// <param name="color">Sprite color</param>
        /// <param name="effect">Sprite effect to apply</param>
        public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos, Color color, SpriteEffects effect)
        {
            Rectangle sourcerect = new Rectangle(Width * (frame % framesPerRow), Height * (frame / framesPerRow),
                Width, Height);
            batch.Draw(spriteSheet, screenPos, sourcerect, color,
                rotation, origin, Scale, effect, Depth);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the frame of the animation
        /// </summary>
        /// <param name="t">GameTime object</param>
        public void Update(GameTime t)
        {
            UpdateFrame((float)t.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Updates the animation. This method will loop the animation.
        /// </summary>
        /// <param name="elapsed">Time elapsed since last update. Usually taken from gameTime.ElapsedGameTime</param>
        public void UpdateFrame(float elapsed)
        {
            if (paused) return;

            TotalElapsed += elapsed;
            
            if (TotalElapsed > TimePerFrame)
            {
                Frame++;
                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                TotalElapsed -= TimePerFrame;
            }
        }

        /// <summary>
        /// Plays through one loop of the animation.
        /// </summary>
        /// <param name="elapsed">Time elapsed since last update.</param>
        /// <returns>False if the image has run out of frames to animate.</returns>
        public bool UpdateFrameOnce(float elapsed)
        {
            if (paused)
                return true;
            TotalElapsed += elapsed;
            if (TotalElapsed > TimePerFrame && Frame < framecount)
            {
                Frame++;
                // Keep the Frame between 0 and the total frames, minus one.
                TotalElapsed -= TimePerFrame;
            }
            return Frame < framecount;
        }

        #endregion

        #region Control Function

        /// <summary>
        /// Resets the animation to the beginning.
        /// </summary>
        public void Reset()
        {
            Frame = 0;
            TotalElapsed = 0f;
        }

        /// <summary>
        /// Stops playing the current animation and resets it to the starting frame.
        /// </summary>
        public void Stop()
        {
            Pause();
            Reset();
        }

        /// <summary>
        /// Unpauses the animation.
        /// </summary>
        public void Play()
        {
            paused = false;
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            paused = true;
        }

        #endregion
    }
}