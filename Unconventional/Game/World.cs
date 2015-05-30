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
        public static int LevelNum = 0;

        public World()
        {
            Image level = new Image(Program.Container.ReadData("level_" + LevelNum.ToString() + ".png"));

            SolidsWidth = level.Width;
            SolidsHeight = level.Height;

            Data = new List<Line>[SolidsWidth];
            for (int i = 0; i < Data.Length; i++)
                Data[i] = new List<Line>();

            for (int x = 0; x < level.Width; x++)
            {
                int beginY = int.MinValue;
                Color beginColor = new Color(0, 0, 0, 0);

                for (int y = 0; y < level.Height; y++)
                {
                    var color = level.GetColor(x, y);

                    if (color == Color.Green)
                    {
                        var pos = new Vector2(x, y);
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            Scene.CreateObject<LevelEnd>(pos);
                        });

                        continue;
                    }
                    else if (color == new Color(255, 0, 255))
                    {
                        var pos = new Vector2(x, y);
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            var player = Scene.CreateObject<Player>(pos);
                            player.World = this;
                            ((MainScene)Scene).Player = player;
                        });

                        continue;
                    }
                    else if (color == Color.Red)
                    {
                        var pos = new Vector2(x, y);
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            Scene.CreateObject<Enemy>(pos);
                        });

                        continue;
                    }

                    if (color != Color.Black)
                    {
                        if (beginY == int.MinValue)
                        {
                            beginColor = color;
                            beginY = y;
                        }
                        else if (color != beginColor)
                        {
                            AddLine(new Line
                            {
                                IsSolid = beginColor == Color.Blue ? false : true,
                                IsStatic = beginColor == Color.Yellow ? true : false,
                                From = beginY,
                                To = y + 1
                            }, x);
                            beginY = int.MinValue;
                        }
                    }
                    else if (beginY != int.MinValue)
                    {
                        AddLine(new Line
                        {
                            IsSolid = beginColor == Color.Blue ? false : true,
                            IsStatic = beginColor == Color.Yellow ? true : false,
                            From = beginY,
                            To = y
                        }, x);
                        beginY = int.MinValue;
                    }
                }

                if (beginY != int.MinValue)
                {
                    AddLine(new Line
                    {
                        IsSolid = beginColor == Color.Blue ? false : true,
                        IsStatic = beginColor == Color.Yellow ? true : false,
                        From = beginY,
                        To = level.Height
                    }, x);
                    beginY = int.MinValue;
                }
            }

            switch (LevelNum)
            {
                case 0:
                    {
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            var hint = Scene.CreateObject<Hint>(new Vector2(529, 224f));
                            hint.HintId = 0;
                            hint = Scene.CreateObject<Hint>(new Vector2(1004f, 164f));
                            hint.HintId = 1;
                            hint = Scene.CreateObject<Hint>(new Vector2(1750f, 250f));
                            hint.HintId = 1;
                        });
                    }
                    break;
                case 1:
                    {
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            var hint = Scene.CreateObject<Hint>(new Vector2(985f, 230f));
                            hint.HintId = 2;
                        });
                    }
                    break;
                case 2:
                    {
                        Engine.InvokeTimed(0f, (offset) =>
                        {
                            var hint = Scene.CreateObject<Hint>(new Vector2(976f, 250f));
                            hint.HintId = 2;
                            hint = Scene.CreateObject<Hint>(new Vector2(1762f, 394f));
                            hint.HintId = 2;
                        });
                    }
                    break;
            }

            RegisterEvent<DrawEvent>(0, Draw);
        }

        public void Draw(DrawEvent ev)
        {
            int start = (int)(Scene.Camera.WorldCoord.X - Engine.Resolution.X / 2f),
                end = (int)(Scene.Camera.WorldCoord.X + Engine.Resolution.X / 2f);
            if (start < 0)
                start = 0;
            if (start >= Data.Length)
                start = Data.Length - 1;
            if (end < 0)
                end = 0;
            if (end >= Data.Length)
                end = Data.Length - 1;

            for (int x = start; x < end; x++)
            {
                var list = Data[x];
                for (int i = 0; i < list.Count; i++)
                {
                    var line = list[i];

                    ev.RenderTarget.DrawTexture(Program.Pixel, new Vector2(x, line.From), line.IsStatic ? Program.Static : Program.Foreground, new Vector2(1f, line.To - line.From), Vector2.Zero, 0f, new Rectangle(Vector2.Zero, Vector2.One));
                }
            }
        }

        public void AddLine(Line line, int x)
        {
            if (!line.IsValid)
                return;
            if (x < 0 || x >= Data.Length)
                return;
            Data[x].Add(line);
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
            if (x2 >= SolidsWidth)
                x2 = SolidsWidth - 1;

            for (int xp = x1; xp <= x2; xp++)
            {
                var list = Data[xp];

                for (int i = 0; i < list.Count; i++)
                {
                    var line = list[i];
                    if (!line.IsSolid)
                        continue;
                    if (y2 > line.From && y1 <= line.To)
                        return false;
                }
            }

            return true;
        }

        public bool IsFree(Vector2 point)
        {
            int x = (int)point.X;
            if (x < 0 || x >= Data.Length)
                return true;

            var list = Data[x];

            for (int i = 0; i < list.Count; i++)
            {
                var line = list[i];
                if (!line.IsSolid)
                    continue;
                if (point.Y > line.From && point.Y <= line.To)
                    return false;
            }
            return true;
        }

        public int Sample(Vector2 point)
        {
            int x = (int)point.X;
            if (x < 0 || x >= Data.Length)
                return 0;

            var list = Data[x];

            for (int i = 0; i < list.Count; i++)
            {
                var line = list[i];
                if (line.IsStatic)
                    continue;
                if (point.Y > line.From && point.Y <= line.To)
                    return line.IsSolid ? 1 : 2;
            }
            return 0;
        }
    }
}
