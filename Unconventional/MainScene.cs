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
        public static bool GameStarted;

        private Vector2 startMove;
        private float moveProgress;

        bool foundGoal;
        bool isFirstStep = true;

        public MainScene()
            : base("Unconventional")
        {
            BackgroundColor = Program.Background;
            Camera.LocalCoord = Engine.Resolution / 2f;

            if (!GameStarted)
            {
                var logo = new LogoElement(Interface, Interface.Size / 2f + new Vector2(0f, 32f));
                var menu = new MenuElement(Interface, new Cog.Vector2(0f, Interface.Size.Y / 2f + logo.Size.Y + 8f), Program.Font16);
                var info = new TextElement(Interface, new Cog.Vector2(Interface.Size.X / 2f, Interface.Size.Y - 8f), Program.Font12,
                    "A game made in 48 hours for Ludum Dare 32 and LBS Game Awards 2015", Cog.Modules.Renderer.HAlign.Center, Cog.Modules.Renderer.VAlign.Bottom);

                InterfaceElements.Add(logo);
                InterfaceElements.Add(menu);
                InterfaceElements.Add(info);

                menu.AddOption("Start Game", () =>
                {
                    GameStarted = true;
                });
                menu.AddOption("Quit", () =>
                {
                    Engine.SceneHost.Pop();
                });
            }

            World = CreateObject<World>(Vector2.Zero);

            if (!GameStarted)
            {
                Camera.WorldCoord += new Vector2(328f, 0f);
                startMove = Camera.WorldCoord;
            }

            if (World.LevelNum > 0)
            {
                moveProgress = -1f;
            }

            RegisterEvent<UpdateEvent>(0, Update);
        }

        private void Update(UpdateEvent ev)
        {
            if (!foundGoal && World.LevelNum > 0)
            {
                var goal = EnumerateObjects<LevelEnd>().FirstOrDefault();
                if (goal != null)
                {
                    startMove = goal.WorldCoord;
                    foundGoal = true;
                }
            }

            if (GameStarted && Player != null && !Player.Enabled)
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
                    if (!isFirstStep)
                        moveProgress += ev.DeltaTime * 0.66f;

                    if (moveProgress < 0f)
                    {
                        Camera.WorldCoord = startMove;
                    }
                    else
                    {
                        Camera.WorldCoord = (startMove + (Player.WorldCoord - startMove) * (Mathf.Sin(-Mathf.HalfPi + moveProgress * Mathf.Pi) + 1f) / 2f - Engine.Resolution / 2f);
                        Camera.WorldCoord = new Vector2((int)Camera.WorldCoord.X, (int)Camera.WorldCoord.Y);
                        Camera.WorldCoord += Engine.Resolution / 2f;
                        var move = (Player.WorldCoord - Camera.WorldCoord).Unit * 200f * ev.DeltaTime;

                        if (moveProgress >= 1f)
                            Player.Enabled = true;
                    }
                }
            }

            if (Player != null && Player.Stun == 0f && Player.Enabled)
            {
                Camera.WorldCoord = Player.WorldCoord;
                var camera = Camera.WorldCoord - Engine.Resolution / 2f;
                if (camera.X < 0f)
                    camera.X = 0f;
                if (camera.X + Engine.Resolution.X > World.SolidsWidth)
                    camera.X = World.SolidsWidth - Engine.Resolution.X;
                Camera.WorldCoord = camera + Engine.Resolution / 2f;
            }

            isFirstStep = false;
        }
    }
}
