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
    class LineElement : InterfaceElement
    {
        public LineElement(InterfaceElement parent, Vector2 location)
            : base(parent, location, 100)
        {
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            target.DrawTexture(Program.Pixel, drawPosition, Program.Foreground, new Vector2(Parent.Size.X, Parent.Size.Y - Location.Y), Vector2.Zero, 0f, new Rectangle(Vector2.Zero, Vector2.One));
            target.DrawTexture(Program.Player, drawPosition + new Vector2(Parent.Size.X / 2f - 128f, -64f), Program.Foreground, Vector2.One, Vector2.Zero, 0f, new Rectangle(Vector2.Zero, new Vector2(40f, 64f)));
            target.DrawTexture(Program.Flower, drawPosition + new Vector2(Parent.Size.X / 2f + 128f - 24f, -64f), Program.Foreground, Vector2.One, Vector2.Zero, 0f, new Rectangle(Vector2.Zero, new Vector2(64f, 64f)));

            //target.DrawTexture(Program.BackgroundTexture, new Vector2(Parent.Size.X / 2f - Program.BackgroundTexture.Size.X / 2f, Parent.Size.Y - Program.BackgroundTexture.Size.Y), Program.Background, Vector2.One, Vector2.Zero, 0f, new Rectangle(Vector2.Zero, Program.BackgroundTexture.Size));
            base.OnDraw(target, drawPosition);
        }
    }
}
