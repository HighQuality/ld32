using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Content;
using Cog.Modules.Renderer;
using Cog;

namespace Unconventional.Game
{
    abstract class ParticleSystem : GameObject
    {
        public bool HasRotation { get; private set; }
        public bool HasScale { get; private set; }
        public BlendMode BlendMode = Engine.Renderer.AlphaBlend;

        private Particle[] particles;
        private int particleCount = 0;

        public bool Emmit = true;

        System.Diagnostics.Stopwatch deltaWatch = System.Diagnostics.Stopwatch.StartNew();

        public ParticleSystem(int capacity)
        {
            particles = new Particle[capacity];

            AddDraw(Draw);
        }

        protected virtual void Draw(DrawEvent ev, DrawTransformation transform)
        {
            float deltaTime = (float)deltaWatch.Elapsed.TotalSeconds;
            deltaWatch.Restart();

            using (var ac = BlendMode.Activate())
            {
                for (int i = 0; i < particleCount; i++)
                {
                    particles[i].Age += deltaTime;

                    if (particles[i].Age >= particles[i].Life)
                    {
                        particles[i] = particles[particleCount - 1];
                        particleCount--;

                        if (i >= particleCount)
                            break;
                    }
                    else
                    {
                        UpdateAndDraw(particles[i], deltaTime, ev.RenderTarget);
                    }
                }
            }
        }

        public void Add(Particle particle)
        {
            int particleIndex = particleCount++;
            if (particleIndex >= particles.Length)
            {
                Debug.Warning("Perf Warning: Had to resize particle system: {0} (Ran out of free slots)", GetType().Name);
                Array.Resize(ref particles, particles.Length * 2);
            }
            particles[particleIndex] = particle;
        }

        public abstract void UpdateAndDraw(Particle particle, float deltaTime, IRenderTarget target);
    }
}
