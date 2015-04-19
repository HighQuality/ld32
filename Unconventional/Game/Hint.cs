using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Content;
using System.Diagnostics;
using Cog.Modules.Renderer;
using Cog;
using Cog.Modules.EventHost;

namespace Unconventional.Game
{
    class Hint : GameObject
    {
        public int HintId;

        private Stopwatch watch;
        private float opacity;

        public Hint()
        {
            watch = Stopwatch.StartNew();
            AddDraw(Draw);

            RegisterEvent<UpdateEvent>(0, Update);
        }

        public void Update(UpdateEvent ev)
        {
            var player = ((MainScene)Scene).Player;
            if (player == null || player.Enabled == false)
                return;

            if ((WorldCoord - player.WorldCoord).Length < 128f)
            {
                opacity += ev.DeltaTime * 5f;
            }
            else
            {
                opacity -= ev.DeltaTime * 5f;
            }

            opacity = Mathf.Max(Mathf.Min(opacity, 1f), 0f);
        }

        public void DrawHint(string text)
        {
        }

        public void Draw(DrawEvent ev, DrawTransformation transform)
        {
            var player = ((MainScene)Scene).Player;
            if(player == null)
                return;

            switch (HintId)
            {
                case 0:
                    ev.RenderTarget.DrawTexture(Program.Hint1, transform.WorldCoord.Floor, Program.Foreground * opacity, Vector2.One, (new Vector2(31f, 21f) / 2f).Floor, 0f, new Rectangle(watch.Elapsed.TotalSeconds % 1f > .5 ? new Vector2(31f, 0f) : new Vector2(0f, 0f), new Vector2(31f, 21f)));
                    break;
                case 1:
                    ev.RenderTarget.DrawTexture(Program.Hint2, transform.WorldCoord.Floor, Program.Foreground * opacity, Vector2.One, (new Vector2(31f, 21f) / 2f).Floor, 0f, new Rectangle(watch.Elapsed.TotalSeconds % 1f > .5 ? new Vector2(31f, 0f) : new Vector2(0f, 0f), new Vector2(31f, 21f)));
                    break;
                case 2:
                    ev.RenderTarget.DrawTexture(Program.Hint3, transform.WorldCoord.Floor, Program.Foreground * opacity, Vector2.One, (new Vector2(9f, 10f) / 2f).Floor, 0f, new Rectangle(watch.Elapsed.TotalSeconds % 1f > .5 ? new Vector2(9f, 0f) : new Vector2(0f, 0f), new Vector2(9f, 10f)));
                    break;
            }
        }
    }
}
