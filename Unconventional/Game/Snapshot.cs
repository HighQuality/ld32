using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Content;
using Cog;
using Cog.Modules.Renderer;
using Cog.Modules.EventHost;

namespace Unconventional.Game
{
    class Snapshot : GameObject
    {
        public SpriteComponent Sprite;
        private float timeSinceSnap;

        public Snapshot()
        {
            Size = new Cog.Vector2(200f, 96f);
            Sprite = SpriteComponent.RegisterOn(this, Program.Pixel);
            Sprite.Origin = new Vector2(0f, 0f);
            Sprite.Color = Program.Foreground;
            Sprite.Color.A = 0;
            Sprite.Scale = Size;

            RegisterEvent<UpdateEvent>(0, Update);
        }

        public void Update(UpdateEvent ev)
        {
            const float openTime = .005f,
                pureTime = .1f,
                fadeTime = .1f;

            if (timeSinceSnap < openTime && timeSinceSnap + ev.DeltaTime >= openTime)
            {
                Cut();
            }

            timeSinceSnap += ev.DeltaTime;
            if (timeSinceSnap < openTime)
            {
                Sprite.Color.A = (int)(255f * Mathf.Sin(timeSinceSnap * 4f * Mathf.HalfPi));
            }
            else if (timeSinceSnap < openTime + pureTime)
            {
                Sprite.Color.A = 255;
            }
            else if (timeSinceSnap < openTime + pureTime + fadeTime)
            {
                Sprite.Color.A = 255 - (int)(255f * Mathf.Sin((timeSinceSnap - openTime - pureTime) * (1f / fadeTime) * Mathf.HalfPi));
            }
            else if (Sprite != null)
            {
                Sprite.Remove();
            }
        }

        public void Cut()
        {
            const bool debugInfo = false;

            Vector2 horizontalUnit = LocalRotation.Unit;
            horizontalUnit *= 1f / horizontalUnit.X;
            horizontalUnit.X = 1f;

            Vector2 verticalUnit = (LocalRotation + Angle.FromDegree(90f)).Unit;
            if (verticalUnit.X != 0f)
            {
                verticalUnit *= 1f / verticalUnit.X;
                verticalUnit.X = 1f;
            }

            Vector2 topLeft = Vector2.Zero;
            Vector2 topRight = new Vector2(Size.X, 0f).Rotate(LocalRotation);
            Vector2 bottomLeft = new Vector2(0f, Size.Y).Rotate(LocalRotation);
            Vector2 bottomRight = Size.Rotate(LocalRotation);

            var world = ((MainScene)Scene).World;

            // kx + m
            float hk = horizontalUnit.Y;
            float vk = verticalUnit.Y;

            float switchPoint;
            if (topLeft.X > bottomLeft.X)
                switchPoint = topLeft.X;
            else
                switchPoint = bottomLeft.X;

            float switchPoint2;
            if (topRight.X < bottomRight.X)
                switchPoint2 = topRight.X;
            else
                switchPoint2 = bottomRight.X;

            float x1 = Mathf.Min(topLeft.X, bottomLeft.X);
            float x2 = Mathf.Max(topRight.X, bottomRight.X);
            
            for (int x = (int)Math.Floor(x1); x < (int)Math.Ceiling(x2); x++)
            {
                var xx = (int)WorldCoord.X + x;
                if (xx < 0 || xx >= world.Data.Length)
                    continue;

                // y = kx + m
                float height;
                float bottom;
                if (x < switchPoint)
                {
                    if (topLeft.X < bottomLeft.X)
                    {
                        height = topLeft.Y + hk * x;
                        bottom = topLeft.Y + vk * x;
                    }
                    else
                    {
                        height = topLeft.Y + vk * x;
                        bottom = bottomLeft.Y + hk * (x - bottomLeft.X);
                    }
                }
                else if (x > switchPoint2)
                {
                    if (topRight.X < bottomRight.X)
                    {
                        height = topRight.Y + vk * (x - topRight.X);
                        bottom = bottomRight.Y + hk * (x - bottomRight.X);
                    }
                    else
                    {
                        height = topLeft.Y + hk * x;
                        bottom = bottomRight.Y + vk * (x - bottomRight.X);
                    }
                }
                else
                {
                    height = topLeft.Y + hk * (x - topLeft.X);
                    bottom = bottomLeft.Y + hk * (x - bottomLeft.X);
                }

                float y1 = (int)LocalCoord.Y + height,
                    y2 = (int)LocalCoord.Y + bottom;

                if (debugInfo)
                {
                    SpriteComponent.RegisterOn(Scene.CreateObject<Node>(new Vector2(WorldCoord.X + x, y1)), Program.Pixel);
                    SpriteComponent.RegisterOn(Scene.CreateObject<Node>(new Vector2(WorldCoord.X + x, y2)), Program.Pixel);
                }

                var list = world.Data[xx];
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    var line = list[i];
                    if (y2 > line.From && y1 <= line.To)
                    {
                        if (y1 <= line.From && y2 >= line.To)
                        {
                            list.RemoveAt(i);
                            i--;
                            count--;
                        }
                        else if (y1 <= line.From)
                            line.From = (int)y2;
                        else if (y2 >= line.To)
                            line.To = (int)y1;
                        else
                        {
                            // This line needs to be split into two seperate ones
                            list.RemoveAt(i);
                            i--;
                            count--;

                            list.Add(new Line
                            {
                                IsSolid = true,
                                From = line.From,
                                To = (int)y1
                            });
                            list.Add(new Line
                            {
                                IsSolid = true,
                                From = (int)y2,
                                To = line.To
                            });
                        }
                    }
                }
            }
        }
    }
}
