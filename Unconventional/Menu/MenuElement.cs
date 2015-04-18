using Cog;
using Cog.Interface;
using Cog.Modules.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;

namespace Unconventional.Menu
{
    class MenuElement : InterfaceElement, IOpacity
    {
        public float Opacity { get; set; }

        public BitmapFont Font;

        public List<string> Options = new List<string>();
        public List<Action> Actions = new List<Action>();
        public List<Vector2> Sizes = new List<Vector2>();

        public MenuElement(InterfaceElement parent, Vector2 location, BitmapFont font)
            : base(parent, location)
        {
            Opacity = 1f;

            this.Font = font;
            Location = new Vector2(0f, Location.Y);
            Size = new Vector2(Parent.Size.X, 0f);
        }

        public void AddOption(string text, Action onClicked)
        {
            Options.Add(text);
            Actions.Add(onClicked);
            Sizes.Add(Font.MeassureString(text, Font.RenderSize));
            Size += new Vector2(0f, Font.RenderLineHeight * 1.5f);
        }

        public override void OnPressed(Mouse.Button button, Vector2 position)
        {
            var drawPosition = ScreenLocation;

            drawPosition.X += Size.X / 2f;
            for (int i = 0; i < Options.Count; i++)
            {
                if (new Rectangle(drawPosition - new Vector2(Sizes[i].X / 2f, 0f), Sizes[i]).Contains(Mouse.Location))
                {
                    Actions[i]();
                    break;
                }
                drawPosition.Y += Font.RenderLineHeight * 1.5f;
            }

            base.OnPressed(button, position);
        }

        public override void OnDraw(IRenderTarget target, Vector2 drawPosition)
        {
            drawPosition.X += Size.X / 2f;
            for (int i=0; i<Options.Count; i++)
            {
                if (new Rectangle(drawPosition - new Vector2(Sizes[i].X / 2f, 0f), Sizes[i]).Contains(Mouse.Location))
                    Font.DrawString(target, "<" + Options[i] + ">", Font.RenderSize, Program.Background * Opacity, drawPosition, HAlign.Center, VAlign.Top);
                else
                    Font.DrawString(target, Options[i], Font.RenderSize, Program.Background * Opacity, drawPosition, HAlign.Center, VAlign.Top);
                drawPosition.Y += Font.RenderLineHeight * 1.5f;
            }

            base.OnDraw(target, drawPosition);
        }
    }
}
