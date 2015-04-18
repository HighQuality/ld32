using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;

namespace Unconventional.Game
{
    class DustSystem : ParticleSystem
    {
        public Texture Texture;
        public Vector2 Origin;

        public Color initialColor,
            endColor;

        public DustSystem()
            : base(64)
        {
            initialColor = Program.Foreground;
            endColor = Program.Foreground;
            endColor.A = 0;

            Depth = 10000;
            BlendMode = Engine.Renderer.AlphaBlend;

            Texture = Program.Pixel;
            Origin = Texture.Size / 2f;
        }

        public override void UpdateAndDraw(Particle particle, float deltaTime, IRenderTarget target)
        {
            particle.Speed += new Vector2(0f, 300f) * deltaTime;
            particle.Color = initialColor.Transition(endColor, particle.Age / particle.Life);
            var backColor = new Color(0, 0, 0, 255).Transition(new Color(0, 0, 0, 0), particle.Age / particle.Life);
            particle.Coordinate += particle.Speed * deltaTime;

            target.DrawTexture(Texture, particle.Coordinate - new Vector2(2f, 2f), backColor, particle.Scale + new Vector2(4f, 4f), Origin, particle.Rotation.Degree, new Rectangle(Vector2.Zero, Texture.Size));
            target.DrawTexture(Texture, particle.Coordinate, particle.Color, particle.Scale, Origin, particle.Rotation.Degree, new Rectangle(Vector2.Zero, Texture.Size));
        }
    }
}
