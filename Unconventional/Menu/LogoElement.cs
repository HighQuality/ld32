using Cog;
using Cog.Interface;
using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog.Modules.Renderer;

namespace Unconventional.Menu
{
    class LogoElement : InterfaceElement, IOpacity
    {
        public float Opacity { get; set; }
        public Texture Texture = Program.Logo;
        public Color Color = Program.Background;

        public LogoElement(InterfaceElement parent, Vector2 location)
            : base(parent, location)
        {
            Opacity = 1f;

            Size = Program.Logo.Size;
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            target.DrawTexture(Texture, drawPosition, Color * Opacity, Vector2.One, (Texture.Size / 2f).Floor, 0f, new Rectangle(Vector2.Zero, Texture.Size));
            base.OnDraw(target, drawPosition);
        }
    }
}
