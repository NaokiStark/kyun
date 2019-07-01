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
            
            Particle particle = new Particle(tx, velocity, startUpPosition, timeToDie, angleVelocity);

            if (!Settings1.Default.MyPCSucks)
                particles.Add(particle);

            return particle;
        }
        public Particle AddNewSquareParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {
            SquareParticle particle = new SquareParticle(tx, velocity, startUpPosition, timeToDie, angleVelocity, pColor);

            if (!Settings1.Default.MyPCSucks)
                particles.Add(particle);

            return particle;
        }

        public Particle AddNewScoreParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {
            ParticleScore particle = new ParticleScore(tx, velocity, startUpPosition, timeToDie, angleVelocity, pColor);

            if (!Settings1.Default.MyPCSucks)
                particles.Add(particle);

            return particle;
        }

        public Particle AddNewHitObjectParticle(Texture2D tx, Vector2 velocity, Vector2 startUpPosition, int timeToDie, float angleVelocity, Color pColor)
        {
            HitObjectParticle particle = new HitObjectParticle(tx, velocity, startUpPosition, timeToDie, angleVelocity) { TextureColor = pColor};

            if (!Settings1.Default.MyPCSucks)
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
            if (Settings1.Default.MyPCSucks)
                return;

            /*
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TimeToDie <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }*/

            foreach (Particle particle in particles)
            {
                particle.Update();
            }

            particles.RemoveAll(p => p.TimeToDie <= 0);


            particles = new List<Particle>(particles.OrderByDescending(p => p.Scale));
        }

        /// <summary>
        /// Draw particles
        /// </summary>
        public override void Render()
        {
            if (Settings1.Default.MyPCSucks)
                return;

            for(int a = 0; a < particles.Count; a++)
            {

                Particle particle = particles[a];

                if (particle is SquareParticle)
                {
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
                    ((Particle)particle).Render();
                }
            }         
        }
    }
}
