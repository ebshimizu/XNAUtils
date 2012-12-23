#region File Info
/*
 * Particle.cs
 * Author: Evan Shimizu, 2012
 * Loosely based off Tigran Gasparian's particle system: http://blog.tigrangasparian.com/2010/10/08/2d-particles-in-xna-part-1-of-3/
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

namespace XNAUtils.support.particles
{
    /// <summary>
    /// Defines the basic unit of a 2D particle system.
    /// Particles maintain their speed and direction. They can also be assigned values for
    /// a variety of acceleration forces. These are typically all the same for a particle emitter,
    /// but the emitter may choose to vary these values for funsies.
    /// </summary>
    class Particle
    {
        #region Fields and Properties

        /// <summary>
        /// Particle Base texture
        /// </summary>
        private Sprite particleImg;

        /// <summary>
        /// Particle position
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Particle speed
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Gravitational force on the particle.
        /// Default 0. Negative gravitational force goes up because up is -y in XNA.
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// Wind accelerates a particle from starting velocity to the wind speed.
        /// If wind is 0, no wind force will be applied. If wind is less than current velocity,
        /// no wind will be applied.
        /// </summary>
        public Vector2 Wind { get; set; }

        /// <summary>
        /// Represents an arbitary external force applied to the particle.
        /// </summary>
        public Vector2 ExternalForce { get; set; }

        /// <summary>
        /// Particle Mass
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Acceleration along particle movement vector.
        /// Negative values make the particle accelerate faster, positive values make the particle slow.
        /// </summary>
        public float Damping { get; set; }

        /// <summary>
        /// Particle scale.
        /// </summary>
        public float Scale { get { return MathHelper.Lerp(endScale, startScale, lifePhase); } }

        /// <summary>
        /// Starting particle scale
        /// </summary>
        private float startScale;

        /// <summary>
        /// Ending particle scale
        /// </summary>
        private float endScale;
        
        /// <summary>
        /// Particle rotation
        /// </summary>
        public float Rotation { get { return particleImg.Rotation; } set { particleImg.Rotation = value; } }

        /// <summary>
        /// Particle color
        /// </summary>
        public Color ParticleColor { get { return Color.Lerp(endColor, startColor, lifePhase); } }

        /// <summary>
        /// Particle start color.
        /// </summary>
        private Color startColor;

        /// <summary>
        /// Particle end color.
        /// </summary>
        private Color endColor;

        /// <summary>
        /// Particle life remaining in seconds.
        /// </summary>
        public float Life { get; set; }

        private static float LifeEpsilon = 0.0001f;

        /// <summary>
        /// Indicates if the particle is alive
        /// </summary>
        public bool Alive { get { return Life > LifeEpsilon; } }

        /// <summary>
        /// Starting life amount.
        /// </summary>
        private float startLife;

        /// <summary>
        /// Returns the life phase. 1.0 = new, 0.0 = dead.
        /// </summary>
        private float lifePhase { get { return Life / startLife; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a particle with just a texture
        /// </summary>
        /// <param name="pTex">Particle image</param>
        public Particle(Sprite pTex)
        {
            init(pTex, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0.0f, 1.0f, 1.0f, Color.White, Color.White, 0.0f, 1.0f);
        }

        /// <summary>
        /// Initialize a particle with position, velocity and life time
        /// </summary>
        /// <param name="pTex">Particle image</param>
        /// <param name="position">Particle Start Position</param>
        /// <param name="velocity">Particle Velocity</param>
        /// <param name="life">Particle life span in seconds</param>
        public Particle(Sprite pTex, Vector2 position, Vector2 velocity, float life, float mass = 1.0f)
        {
            init(pTex, position, velocity, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0.0f, 1.0f, 1.0f, Color.White, Color.White, life, mass);
        }

        /// <summary>
        /// Initialize a particle using all available parameters
        /// </summary>
        /// <param name="pTex">Particle Image</param>
        /// <param name="position">Initial position</param>
        /// <param name="velocity">Initial velocity</param>
        /// <param name="gravity">Gravitational acceleration</param>
        /// <param name="wind">Wind force</param>
        /// <param name="extforce">External force</param>
        /// <param name="damping">Speed damping</param>
        /// <param name="scale">Particle scale</param>
        /// <param name="color">Particle color</param>
        /// <param name="life">Particle life span in seconds</param>
        public Particle(Sprite pTex, Vector2 position, Vector2 velocity, Vector2 gravity, Vector2 wind, Vector2 extForce, float damping, float scaleStart,
                        float scaleEnd, Color colorStart, Color colorEnd, float life, float mass = 1.0f)
        {
            init(pTex, position, velocity, gravity, wind, extForce, damping, scaleStart, scaleEnd, colorStart, colorEnd, life, mass);
        }

        /// <summary>
        /// Shared init method to assist particle construction
        /// </summary>
        public void init(Sprite pTex, Vector2 position, Vector2 velocity, Vector2 gravity, Vector2 wind, Vector2 extForce,
                          float damping, float scaleBegin, float scaleEnd, Color colorBegin, Color colorEnd, float life, float mass)
        {
            particleImg = pTex;
            Position = position;
            Velocity = velocity;
            Gravity = gravity;
            Wind = wind;
            ExternalForce = extForce;
            Damping = damping;
            startColor = colorBegin;
            endColor = colorEnd;
            startScale = scaleBegin;
            endScale = scaleEnd;
            Life = life;
            startLife = life;
            Mass = mass;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update a particle's position.
        /// </summary>
        /// <param name="dt">Time since last update in seconds</param>
        /// <returns>true if particle is alive after update, false otherwise.</returns>
        public bool Update(float dt)
        {
            // Update velocity based on forces.
            Velocity += (Gravity / Mass) * dt;
            Velocity += (ExternalForce / Mass) * dt;

            if (Velocity.Length() < Wind.Length())
                Velocity += ((Wind - Velocity) / Mass) * dt;

            if (Damping != 0.0f && Velocity.Length() > 0.1f)
            {
                Vector2 vNorm = new Vector2(Velocity.X, Velocity.Y);
                vNorm.Normalize();
                Velocity += (vNorm * Damping) / Mass * dt;
            }

            // Update position based on velocity.
            Position += Velocity * dt;
            Life -= dt;
            if (Alive)
                return true;
            return false;
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the particle.
        /// </summary>
        /// <param name="sb">SpriteBatch for drawing</param>
        /// <param name="offset">Location of the emitter</param>
        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            particleImg.Scale = Scale;
            particleImg.Draw(sb, Position + offset, ParticleColor);
        }

        #endregion
    }
}
