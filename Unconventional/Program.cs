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

namespace Unconventional
{
    static class Program
    {
        public static ResourceContainer Container;

        public static Texture Logo,
            Pixel,
            Player,
            Flower,
            SlopeLeft,
            SlopeRight;
        
        public static BitmapFont Font12,
            Font16;

        public static Color Foreground,
            Background;

        static void Main(string[] args)
        {
            Background = Color.Black;
            Foreground = new Color(253, 227, 167);

            Engine.Initialize<SfmlRenderer, SfmlAudioModule>();
            
            Engine.EventHost.RegisterEvent<InitializeEvent>(0, (ev) =>
            {
                Container = Engine.ResourceHost.LoadDictionary("main", "Resources");
                Font12 = LoadFont("Fonts/merriweather_12.fnt");
                Font16 = LoadFont("Fonts/merriweather_16.fnt");
                Logo = LoadTexture("logo.png");
                Player = LoadTexture("player.png");
                Flower = LoadTexture("flower.png");
                SlopeLeft = LoadTexture("slope_left.png");
                SlopeRight = LoadTexture("slope_right.png");

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
    }
}
