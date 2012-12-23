#region File Info
/*
 * Emitter.cs
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
    /// Particle emitter class. Given an origin and an area to generate particles in, will generate and update particles
    /// and can manipulate them as well.
    /// </summary>
    class Emitter
    {
        #region Fields

        /// <summary>
        /// Image for this particular emitter.
        /// </summary>
        public Sprite baseParticle;

        /// <summary>
        /// Maximum number of active particles.
        /// </summary>
        public int maxActive;

        /// <summary>
        /// Length of time to next particle spawn.
        /// </summary>
        private float nextSpawnTime;

        /// <summary>
        /// Time elapsed since last spawn time.
        /// </summary>
        private float timeSinceLastSpawn;

        /// <summary>
        /// List of active particles.
        /// </summary>
        public List<Particle> activeParticles;

        /// <summary>
        /// Random number generator.
        /// </summary>
        public Random rng;

        /// <summary>
        /// Spawn rate range, x is shortest time, y is longest time.
        /// </summary>
        public Vector2 spawnRate;

        /// <summary>
        /// Direction for spawned particle.
        /// </summary>
        public Vector2 spawnDirection;

        /// <summary>
        /// Varies the spawn direction
        /// </summary>
        public Vector2 spawnNoiseAngle;

        /// <summary>
        /// Sets the mminimum and maximum lifespan for the particle.
        /// </summary>
        public Vector2 lifeRange;

        /// <summary>
        /// Range for the starting scale for the particle.
        /// </summary>
        public Vector2 startScale;

        /// <summary>
        /// Range for the ending scale for the particle
        /// </summary>
        public Vector2 endScale;

        /// <summary>
        /// Starting color 1. Interpolates between Color 1 and 2 for variance.
        /// </summary>
        public Color startColor1;

        /// <summary>
        /// Starting color 2.
        /// </summary>
        public Color startColor2;

        /// <summary>
        /// Ending color 1
        /// </summary>
        public Color endColor1;

        /// <summary>
        /// Ending color 2
        /// </summary>
        public Color endColor2;

        /// <summary>
        /// Range for particle starting speed.
        /// </summary>
        public Vector2 startSpeed;

        /// <summary>
        /// Particle mass variation range
        /// </summary>
        public Vector2 massRange;

        /// <summary>
        /// Emitter position
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// Tells the emitter when to create new particles.
        /// </summary>
        public bool emitting;

        #endregion

        #region Properties

        /// <summary>
        /// Emitter gravity
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// Emitter wind
        /// </summary>
        public Vector2 Wind { get; set; }

        /// <summary>
        /// The external force being applied to all particles.
        /// </summary>
        public Vector2 ExternalForce { get; set; }

        /// <summary>
        /// Particle movement damping
        /// </summary>
        public float Damping { get; set; }

        /// <summary>
        /// Index to the next spawned particle.
        /// </summary>
        private int nextSpawn;
        
        /// <summary>
        /// Count of the number of particles alive.
        /// </summary>
        private int particlesAlive;

        /// <summary>
        /// Returns true if all particles in this emitter are inactive
        /// </summary>
        public bool AllParticlesDead { get { return particlesAlive == 0; } }

        /// <summary>
        /// Returns true if the emitter is not emitting particles and all particles are dead.
        /// </summary>
        public bool Inactive { get { return (!emitting && AllParticlesDead); } } 

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a particle emitter.
        /// </summary>
        /// <param name="baseParticle">Particle image</param>
        /// <param name="maxActive">Maximum number of active particles in the emitter</param>
        /// <param name="rng">Random number generator for noise</param>
        /// <param name="spawnRate">Range of spawn times</param>
        /// <param name="spawnDirection">Base direction for the emitter.</param>
        /// <param name="spawnNoiseAngle">Emission spread</param>
        /// <param name="lifeRange">Lifespan of particles</param>
        /// <param name="startScale">Beginning scale range for the particles.</param>
        /// <param name="endScale">End scale range for the particles</param>
        /// <param name="start1">Start color 1. Lerped with start color 2.</param>
        /// <param name="start2">Start color 2</param>
        /// <param name="end1">End color 1. Lerped with end color 2.</param>
        /// <param name="end2">End color 2</param>
        /// <param name="startSpeed">Range of initial particle speeds</param>
        /// <param name="massRange">Range of particle masses</param>
        /// <param name="gravity">Initial gravity force</param>
        /// <param name="wind">Initial wind force</param>
        /// <param name="externalForce">Initial external force.</param>
        /// <param name="damping">Initial particle drag.</param>
        /// <param name="position">Emitter position</param>
        public Emitter(Sprite baseParticle, int maxActive, Random rng, Vector2 spawnRate, Vector2 spawnDirection, Vector2 spawnNoiseAngle,
                       Vector2 lifeRange, Vector2 startScale, Vector2 endScale, Color start1, Color start2, Color end1, Color end2,
                       Vector2 startSpeed, Vector2 massRange, Vector2 gravity, Vector2 wind, Vector2 externalForce, float damping,
                       Vector2 position, bool emitting = true)
        {
            this.baseParticle = baseParticle;
            this.maxActive = maxActive;
            this.rng = rng;
            this.spawnRate = spawnRate;
            this.spawnDirection = spawnDirection;
            this.spawnNoiseAngle = spawnNoiseAngle;
            this.lifeRange = lifeRange;
            this.startScale = startScale;
            this.endScale = endScale;
            this.startColor1 = start1;
            this.startColor2 = start2;
            this.endColor1 = end1;
            this.endColor2 = end2;
            this.startSpeed = startSpeed;
            this.massRange = massRange;
            this.position = position;
            this.emitting = emitting;
            Gravity = gravity;
            Wind = wind;
            ExternalForce = externalForce;
            Damping = damping;
            activeParticles = new List<Particle>(maxActive);

            particlesAlive = 0;
            nextSpawn = 0;

            // Init particle memory
            for (int i = 0; i < maxActive; i++)
                activeParticles.Add(new Particle(baseParticle));

            nextSpawnTime = MathHelper.Lerp(spawnRate.X, spawnRate.Y, (float)rng.NextDouble());
            timeSinceLastSpawn = 0.0f;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the particles managed by the emitter and spawns new particles if able.
        /// You can subclass the emitter and add updates to external forces, etc. or you can
        /// control those from outside the instance (a wrapper class)
        /// </summary>
        /// <param name="t">GameTime object</param>
        public virtual void Update(GameTime t)
        {
            float dt = (float) t.ElapsedGameTime.TotalSeconds;
            timeSinceLastSpawn += dt;
            if (emitting)
            {
                while (timeSinceLastSpawn > nextSpawnTime)
                {
                    // Spawning time!
                    if (particlesAlive < maxActive)
                    {
                        Vector2 startDir = Vector2.Transform(spawnDirection, Matrix.CreateRotationZ(MathHelper.Lerp(spawnNoiseAngle.X, spawnNoiseAngle.Y, (float)rng.NextDouble())));
                        startDir.Normalize();
                        startDir *= MathHelper.Lerp(startSpeed.X, startSpeed.Y, (float)rng.NextDouble());
                        activeParticles[nextSpawn].init(baseParticle, Vector2.Zero, startDir, Gravity, Wind, ExternalForce, Damping,
                            MathHelper.Lerp(startScale.X, startScale.Y, (float)rng.NextDouble()),
                            MathHelper.Lerp(endScale.X, endScale.Y, (float)rng.NextDouble()),
                            Color.Lerp(startColor1, startColor2, (float)rng.NextDouble()),
                            Color.Lerp(endColor1, endColor2, (float)rng.NextDouble()),
                            MathHelper.Lerp(lifeRange.X, lifeRange.Y, (float)rng.NextDouble()),
                            MathHelper.Lerp(massRange.X, massRange.Y, (float)rng.NextDouble())
                        );
                        nextSpawn = (nextSpawn >= (maxActive - 1)) ? 0 : nextSpawn + 1;
                        particlesAlive++;
                    }
                    timeSinceLastSpawn -= nextSpawnTime;
                    nextSpawnTime = MathHelper.Lerp(spawnRate.X, spawnRate.Y, (float)rng.NextDouble());

                    if (particlesAlive < maxActive)
                    {
                        while (activeParticles[nextSpawn].Alive)
                        {
                            nextSpawn++;
                            if (nextSpawn >= maxActive) nextSpawn = 0;
                        }
                    }
                }
            }

            // Update all particles. Dead particles are  marked as such and are reused.
            foreach (Particle p in activeParticles)
            {
                if (p.Alive && !p.Update(dt))
                    particlesAlive--;
            }
            
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws all the particles managed by this emitter.
        /// </summary>
        /// <param name="sb">SpriteBatch object</param>
        public void Draw(SpriteBatch sb)
        {
            foreach (Particle p in activeParticles)
            {
                if (p.Alive)
                    p.Draw(sb, position);
            }
        }

        /// <summary>
        /// Removes all active particles from this emitter
        /// </summary>
        public void clear()
        {
            activeParticles.Clear();
        }

        #endregion

    }
}
