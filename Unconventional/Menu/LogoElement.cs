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

        public LogoElement(InterfaceElement parent, Vector2 location)
            : base(parent, location)
        {
            Opacity = 1f;

            Location -= new Vector2(Program.Logo.Size.X / 2f, 0f);
            Location = Location.Floor;
            Size = Program.Logo.Size;
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            target.DrawTexture(Program.Logo, drawPosition, Program.Background * Opacity, Vector2.One, Vector2.Zero, 0f, new Rectangle(Vector2.Zero, Program.Logo.Size));
            base.OnDraw(target, drawPosition);
        }
    }
}
