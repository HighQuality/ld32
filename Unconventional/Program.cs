using Cog;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog.Modules.Resources;
using Cog.SfmlAudio;
using Cog.SfmlRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unconventional.Game;
using Cog.Modules.Audio;

namespace Unconventional
{
    static class Program
    {
        public static ResourceContainer Container;

        public static Texture Logo,
            Pixel,
            Player,
            Portal,
            Hint1,
            Hint2,
            Hint3,
            EndScreen;

        public static SoundEffect Music,
            Cut,
            EnemyHit,
            Ground;

        public static BitmapFont Font12,
            Font16;

        public static Color Foreground,
            Static,
            Background;

        static void Main(string[] args)
        {
            Background = Color.Black;
            Static = new Color(52, 73, 94);
            Foreground = new Color(253, 227, 167);

            Engine.Initialize<SfmlRenderer, SfmlAudioModule>();

            Engine.EventHost.RegisterEvent<KeyDownEvent>((int)Keyboard.Key.Escape, 0, (ev) =>
                {
                    while (Engine.SceneHost.CurrentScene != null)
                        Engine.SceneHost.Pop();
                });

            Engine.EventHost.RegisterEvent<InitializeEvent>(0, (ev) =>
            {
                Container = Engine.ResourceHost.LoadDictionary("main", "Resources");
                Font12 = LoadFont("Fonts/merriweather_12.fnt");
                Font16 = LoadFont("Fonts/merriweather_16.fnt");
                Logo = LoadTexture("logo.png");
                Player = LoadTexture("player.png");
                Portal = LoadTexture("portal.png");
                Hint1 = LoadTexture("hint1.png");
                Hint2 = LoadTexture("hint2.png");
                Hint3 = LoadTexture("hint3.png");
                EndScreen = LoadTexture("endscreen.png");

                /*Music = LoadSound("music.ogg");
                EnemyHit = LoadSound("ehit.wav");
                Cut = LoadSound("cut.wav");
                Ground = LoadSound("ground.wav");

                Music.Play();*/

                Image image = new Image(1, 1);
                image.SetColor(0, 0, Color.White);
                Pixel = Engine.Renderer.TextureFromImage(image);

                var scene = Engine.SceneHost.CreateGlobal<MainScene>();
                Engine.SceneHost.Push(scene);
            });

            Engine.StartGame("cut", WindowStyle.Default);
        }

        static BitmapFont LoadFont(string location)
        {
            return (BitmapFont)Container.Load(location);
        }

        static Texture LoadTexture(string location)
        {
            return (Texture)Container.Load(location);
        }

        static SoundEffect LoadSound(string location)
        {
            return (SoundEffect)Container.Load(location);
        }
    }
}
