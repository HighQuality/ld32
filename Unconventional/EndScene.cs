using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Scenes;
using Unconventional.Menu;

namespace Unconventional
{
    class EndScene : Scene
    {
        public EndScene()
            : base("end")
        {
            BackgroundColor = Program.Foreground;
            var logo = new LogoElement(Interface, Interface.Size / 2f);
            logo.Texture = Program.EndScreen;
            logo.Color = Program.Foreground;
        }
    }
}
