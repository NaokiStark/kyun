using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kyun.game;

namespace kyun.GameScreen.UI.Particles
{
    public class ParticleEngine : UIObjectBase
    {

        public List<Particle> particles = new List<Particle>();

        public int ParticleCount
        {
            get
            {
                return particles.Count;
            }
        }

        /// <summary>
        /// Initialize ParticleEngine
        /// </summary>
        public ParticleEngine()
        {

        }

        /// <summary>
        /// Adds New particle
        /// </summary>
        /// <param name="tx">Texture</param>
        /// <param name="velocity">Velocity of Particle</param>
        /// <param name="startUpPosition">Startup position</param>
        /// <param name="timeToDie">Time alive</param>
        /// <param name="angleVelocity">Angle rotation velocity</param>
        /// <returns>A new Particle instance</returns>
        public Particle AddNewParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity)
        {
            if (!Settings1.Default.MyPCSucks)
            {
                Particle particle = new Particle(tx, velocity, startUpPosition, timeToDie, angleVelocity);


                particles.Add(particle);

                return particle;
            }
            return null;
        }
        public Particle AddNewSquareParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {
            if (!Settings1.Default.MyPCSucks)
            {
                SquareParticle particle = new SquareParticle(tx, velocity, startUpPosition, timeToDie, angleVelocity, pColor);

                particles.Add(particle);

                return particle;
            }

            return null;
        }

        public Particle AddNewScoreParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {
            ParticleScore particle = new ParticleScore(tx, velocity, startUpPosition, timeToDie, angleVelocity, pColor);

            particles.Add(particle);

            return particle;
        }

        public Particle AddNewHitObjectParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {

            HitObjectParticle particle = new HitObjectParticle(tx, velocity, startUpPosition, timeToDie, angleVelocity) { TextureColor = pColor };


            particles.Add(particle);

            return particle;
        }

        public void Clear()
        {
            particles.Clear();
        }

        /// <summary>
        /// Updates Particles
        /// </summary>
        public override void Update()
        {
           
            foreach (Particle particle in particles)
            {
                if(particle is ParticleScore || particle is HitObjectParticle)
                {
                    particle.Update();
                }
                else if (!Settings1.Default.MyPCSucks)
                {
                    particle.Update();
                }
            }

            particles.RemoveAll(p => p.TimeToDie <= 0);


            particles = new List<Particle>(particles.OrderByDescending(p => p.Scale));
        }

        /// <summary>
        /// Draw particles
        /// </summary>
        public override void Render()
        {


            for (int a = 0; a < particles.Count; a++)
            {

                Particle particle = particles[a];

                if (particle is SquareParticle)
                {
                    if (Settings1.Default.MyPCSucks)
                    {
                        continue;
                    }

                    ((SquareParticle)particle).Render();
                }
                else if (particle is ParticleScore)
                {
                    ((ParticleScore)particle).Render();
                }
                else if (particle is HitObjectParticle)
                {
                    ((HitObjectParticle)particle).Render();
                }
                else
                {
                    if (Settings1.Default.MyPCSucks)
                    {
                        continue;
                    }
                    ((Particle)particle).Render();
                }
            }
        }
    }
}
