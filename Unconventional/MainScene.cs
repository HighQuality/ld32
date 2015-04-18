using Cog;
using Cog.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unconventional.Game;
using Unconventional.Menu;
using Cog.Modules.EventHost;

namespace Unconventional
{
    class MainScene : Scene
    {
        public Player Player;
        public World World;
        public List<IOpacity> InterfaceElements = new List<IOpacity>();
        public bool gameStarted;

        public MainScene()
            : base("Unconventional")
        {
            BackgroundColor = Program.Background;
            Camera.LocalCoord = Engine.Resolution / 2f;

            var logo = new LogoElement(Interface, Interface.Size / 2f - new Vector2(0f, 14f));
            var menu = new MenuElement(Interface, new Cog.Vector2(0f, Interface.Size.Y / 2f + logo.Size.Y + 8f), Program.Font16);
            var info = new TextElement(Interface, new Cog.Vector2(Interface.Size.X / 2f - 18f, Interface.Size.Y - 8f), Program.Font12,
                "A game made in 48 hours for LD32 and LBS Game Awards 2015", Cog.Modules.Renderer.HAlign.Center, Cog.Modules.Renderer.VAlign.Bottom);

            InterfaceElements.Add(logo);
            InterfaceElements.Add(menu);
            InterfaceElements.Add(info);

            menu.AddOption("Start Game", () =>
            {
                gameStarted = true;
            });
            menu.AddOption("About", () =>
            {
                Engine.SceneHost.Pop();
            });
            menu.AddOption("Quit", () =>
            {
                Engine.SceneHost.Pop();
            });

            World = CreateObject<World>(Vector2.Zero);
            Player = CreateObject<Player>(new Vector2(528f, 0f));
            Player.World = World;
            /*while (Player.IsFree(new Vector2(0f, 1f)))
            {
                Player.LocalCoord += new Vector2(0f, 1f);
            }*/

            Camera.WorldCoord += new Vector2(328f, 0f);

            RegisterEvent<UpdateEvent>(0, Update);
        }

        private void Update(UpdateEvent ev)
        {
            if (gameStarted && !Player.Enabled)
            {
                for (int i=InterfaceElements.Count - 1; i >= 0; i--)
                {
                    InterfaceElements[i].Opacity -= ev.DeltaTime;
                    if (InterfaceElements[i].Opacity <= 0f)
                    {
                        InterfaceElements[i].Remove();
                        InterfaceElements.RemoveAt(i);
                    }
                }

                if (InterfaceElements.Count == 0)
                {
                    var move = (Player.WorldCoord - Camera.WorldCoord).Unit * 200f * ev.DeltaTime;
                    if ((Player.WorldCoord - Camera.WorldCoord).Length < move.Length)
                    {
                        Player.Enabled = true;
                    }
                    else
                    {
                        Camera.WorldCoord += move;
                    }
                }
            }

            if (Player.Enabled)
            {
                Camera.WorldCoord = Player.WorldCoord;
                var camera = Camera.WorldCoord - Engine.Resolution / 2f;
                if (camera.X < 0f)
                    camera.X = 0f;
                if (camera.X + Engine.Resolution.X > World.SolidsWidth * 32f)
                    camera.X = World.SolidsWidth * 32f - Engine.Resolution.X;
                Camera.WorldCoord = camera + Engine.Resolution / 2f;
            }
        }
    }
}
