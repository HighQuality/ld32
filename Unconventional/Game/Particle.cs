using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog;

namespace Unconventional.Game
{
    class Particle
    {
        public Vector2 Coordinate,
            Speed,
            Scale = Vector2.One;
        public Color Color;

        public float Age,
            Life;

        public Angle Rotation;
    }
}
