#region File Info
/*
 * AnimatedText.cs
 * Author: Evan Shimizu, 2012
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
using XNAUtils.support;

namespace XNAUtils.support
{
    /// <summary>
    /// Class that will animate a block of text using simple start and end values for each parameter
    /// </summary>
    class AnimatedText
    {
        #region Fields

        /// <summary>
        /// Start location
        /// </summary>
        private Vector2 start;

        /// <summary>
        /// End location
        /// </summary>
        private Vector2 end;

        /// <summary>
        /// Starting and Ending scale
        /// </summary>
        private Vector2 scale;

        /// <summary>
        /// Starting and ending rotation
        /// </summary>
        private Vector2 rotation;

        /// <summary>
        /// Starting color
        /// </summary>
        private Color startColor;

        /// <summary>
        /// Ending color
        /// </summary>
        private Color endColor;

        /// <summary>
        /// Starting amount of life the text object has
        /// </summary>
        private float startLife;

        /// <summary>
        /// Selected font.
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// The text to be animated
        /// </summary>
        private string text;

        /// <summary>
        /// Text origin.
        /// </summary>
        private Vector2 origin;

        #endregion

        #region Properties

        /// <summary>
        /// Current life of the text object
        /// </summary>
        public float Life { get; set; }

        /// <summary>
        /// Animation phase. 1.0 = start, 0.0 = end
        /// </summary>
        public float LifePhase { get { return Life / startLife; } }

        /// <summary>
        /// Floating point limit for if the text is still active
        /// </summary>
        private static float LifeEpsilon = 0.0001f;

        /// <summary>
        /// Returns true if the text is still actively animating
        /// </summary>
        public bool Alive { get { return LifePhase > LifeEpsilon; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs an animated text object
        /// </summary>
        /// <param name="text">Text to be animated</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="scale">Start and end scale</param>
        /// <param name="rotation">Start and end rotation</param>
        /// <param name="startColor">Start color</param>
        /// <param name="endColor">End color</param>
        /// <param name="life">Duration of animation</param>
        /// <param name="font">Sprite font to use for animating</param>
        public AnimatedText(string text, Vector2 start, Vector2 end, Vector2 scale, Vector2 rotation, Color startColor,
            Color endColor, float life, SpriteFont font)
        {
            this.font = font;
            this.text = text;
            this.start = start;
            this.end = end;
            this.scale = scale;
            this.rotation = rotation;
            this.startColor = startColor;
            this.endColor = endColor;
            this.Life = life;
            this.startLife = life;

            origin = font.MeasureString(text) / 2.0f;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the text life phase
        /// </summary>
        /// <param name="dt">Time since last update (seconds)</param>
        public void Update(float dt)
        {
            Life -= dt;
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the text at the proper location given its parameters
        /// </summary>
        /// <param name="sb">Drawing object</param>
        public void Draw(SpriteBatch sb)
        {
            Vector2 position = Vector2.Lerp(end, start, LifePhase);
            Color color = Color.Lerp(endColor, startColor, LifePhase);
            float currentScale = MathHelper.Lerp(scale.Y, scale.X, LifePhase);
            float currentRotation = MathHelper.Lerp(rotation.Y, rotation.X, LifePhase);

            sb.DrawString(font, text, position, color, currentRotation, origin, currentScale, SpriteEffects.None, 1.0f);
        }

        #endregion

    }
}
