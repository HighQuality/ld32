using Cog;
using Cog.Modules.Content;
using Cog.Modules.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.EventHost;

namespace Unconventional.Game
{
    class World : GameObject
    {
        public List<Line>[] Data;
        public int SolidsWidth;
        public int SolidsHeight;
        public const float SolidsSize = 32f;
        
        public List<Vector2> SpawnPoints = new List<Vector2>();

        public World()
        {
            Image level = new Image(Program.Container.ReadData("level.png"));

            SolidsWidth = level.Width;
            SolidsHeight = level.Height;

            Data = new List<Line>[SolidsWidth * 32];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = new List<Line>();

            for (int x = 0; x < level.Width; x++)
            {
                for (int y = 0; y < level.Height; y++)
                {
                    var color = level.GetColor(x, y);

                    if (color == Color.White)
                    {
                        CreateWall(new Vector2(x * 32f, y * 32), new Vector2(32f, 32f));
                    }
                    else if (color == Color.Red)
                    {
                        CreateSlope(false, new Vector2(x * 32f, y * 32), new Vector2(32f, 32f));
                    }
                    else if (color == Color.Green)
                    {
                        CreateSlope(true, new Vector2(x * 32f, y * 32), new Vector2(32f, 32f));
                    }
                    else if (color == Color.Blue)
                    {
                        CreateDecoration(new Vector2(x * 32f + 16f, y * 32f + 32f));
                    }
                }
            }

            RegisterEvent<KeyDownEvent>((int)Keyboard.Key.Escape, 0, (ev) =>
                {
                    var img = new Image(SolidsWidth * 32, SolidsHeight * 32);
                    for (int y = 0; y < SolidsHeight * 32; y++)
                    {
                        for (int x = 0; x < SolidsWidth * 32; x++)
                        {
                            if (!IsFree(new Rectangle(new Vector2(x, y), Vector2.One)))
                            {
                                img.SetColor(x, y, Color.Black);
                            }
                        }
                    }
                    img.ToBitmap().Save("output.png");
                });
            RegisterEvent<DrawEvent>(0, Draw);
        }

        public void Draw(DrawEvent ev)
        {
            int start = (int)(Scene.Camera.WorldCoord.X - Engine.Resolution.X / 2f),
                end = (int)(Scene.Camera.WorldCoord.X + Engine.Resolution.X / 2f);

            for (int x = start; x < end; x++)
            {
                var list = Data[x];
                for (int i = 0; i < list.Count; i++)
                {
                    var line = list[i];
                    ev.RenderTarget.DrawTexture(Program.Pixel, new Vector2(x, line.From), Program.Foreground, new Vector2(1f, line.To - line.From), Vector2.Zero, 0f, new Rectangle(Vector2.Zero, Vector2.One));
                }
            }
        }

        public void AddLine(Line line, int x)
        {
            Data[x].Add(line);
        }

        public void CreateWall(Vector2 position, Vector2 size)
        {
            /*var block = Scene.CreateLocalObject<Block>(position + size / 2f);
            block.Size = size;
            block.Mask = Program.BlockMask;
            var sc = SpriteComponent.RegisterOn(block, Program.Pixel);
            sc.Color = Program.Foreground;
            sc.Scale = size;
            sc.Origin = new Vector2(.5f, .5f);*/

            for (int x = (int)position.X; x < (int)(position.X + size.X); x++)
            {
                AddLine(new Line
                {
                    IsSolid = true,
                    From = (int)position.Y,
                    To = (int)(position.Y + size.Y)
                }, x);
            }

            //AddSolid(block);
        }

        public void CreateDecoration(Vector2 position)
        {
            /*var node = Scene.CreateLocalObject<Node>(position);
            var decoration = Program.Flower;
            var sc = SpriteComponent.RegisterOn(node, decoration);
            sc.Origin = new Vector2(decoration.Size.X / 2f, decoration.Size.Y);
            sc.Color = Program.Foreground;*/
        }

        public void CreateSlope(bool isLeft, Vector2 position, Vector2 size)
        {
            if (isLeft)
            {
                for (int x = (int)position.X; x < (int)(position.X + size.X); x++)
                {
                    AddLine(new Line
                    {
                        IsSolid = true,
                        From = (int)(position.Y + size.Y * ((x - position.X) / size.X)),
                        To = (int)(position.Y + size.Y)
                    }, x);
                }
            }
            else
            {
                for (int x = (int)position.X; x < (int)(position.X + size.X); x++)
                {
                    AddLine(new Line
                    {
                        IsSolid = true,
                        From = (int)(position.Y + size.Y - size.Y * ((x - position.X) / size.X)),
                        To = (int)(position.Y + size.Y)
                    }, x);
                }
            }
        }

        public bool IsFree(Rectangle rect)
        {
            int x1, y1, x2, y2;

            x1 = (int)Math.Floor(rect.Left);
            y1 = (int)Math.Floor(rect.Top);
            x2 = (int)Math.Floor(rect.Right);
            y2 = (int)Math.Floor(rect.Bottom);

            if (x1 < 0)
                x1 = 0;
            if (x2 >= SolidsWidth * 32)
                x2 = SolidsWidth * 32 - 1;
            if (y1 < 0)
                y1 = 0;
            if (y2 >= SolidsHeight * 32)
                y2 = SolidsHeight * 32 - 1;

            for (int xp = x1; xp <= x2; xp++)
            {
                var list = Data[xp];

                for (int i = 0; i < list.Count; i++)
                {
                    var line = list[i];
                    if (y2 > line.From && y1 <= line.To)
                        return false;
                }
            }

            return true;
        }
    }
}
