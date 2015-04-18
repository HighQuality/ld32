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
    class TextElement : InterfaceElement, IOpacity
    {
        public float Opacity { get; set; }

        public BitmapFont Font;
        public string Text;

        public HAlign hAlign;
        public VAlign vAlign;
        
        public TextElement(InterfaceElement parent, Vector2 location, BitmapFont font, string text, HAlign h, VAlign v)
            : base(parent, location)
        {
            Opacity = 1f;

            this.Text = text;
            this.Font = font;
            this.hAlign = h;
            this.vAlign = v;
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            Font.DrawString(target, Text, Font.RenderSize, Program.Background * Opacity, drawPosition, hAlign, vAlign);

            base.OnDraw(target, drawPosition);
        }
    }
}
