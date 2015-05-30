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
        private int[] terrainData;
        public bool IsFlipped;
        private Player player;
        private Snapshot oldSnap;
        private List<Rectangle> Enemies = new List<Rectangle>();

        public Snapshot()
        {
            Size = new Cog.Vector2(256f, 96f);
            Sprite = SpriteComponent.RegisterOn(this, Program.Pixel);
            Sprite.Origin = new Vector2(0f, 0f);
            Sprite.Color = Program.Foreground;
            Sprite.Color.A = 0;
            Sprite.Scale = Size;

            RegisterEvent<UpdateEvent>(0, Update);
        }

        public override void CreationData(object[] creationData)
        {
            oldSnap = (Snapshot)creationData[0];
            player = (Player)creationData[1];

            base.CreationData(creationData);
        }

        public void Update(UpdateEvent ev)
        {
            const float openTime = .005f,
                pureTime = .1f,
                fadeTime = .1f;

            if (timeSinceSnap < openTime && timeSinceSnap + ev.DeltaTime >= openTime)
            {
                Cut();

                if (oldSnap != null)
                {
                    oldSnap.Paste();
                    if (!player.IsFree(new Vector2(0f, 0f)))
                    {
                        int move = 1;
                        for (; ; )
                        {
                            if (player.IsFree(new Vector2(move, 0f)))
                            {
                                player.LocalCoord += new Vector2(move, 0f);
                                break;
                            }
                            if (player.IsFree(new Vector2(-move, 0f)))
                            {
                                player.LocalCoord += new Vector2(-move, 0f);
                                break;
                            }
                            move++;
                        }
                    }
                }
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

        bool initialFlipped;

        public void Paste()
        {
            if (terrainData == null)
                throw new Exception("not cut yet");

            var world = ((MainScene)Scene).World;

            for (int x = 0; x < (int)Size.X; x++)
            {
                int beginY = int.MinValue;
                int beginType = 0;
                for (int y = 0; y < (int)Size.Y; y++)
                {
                    int terrainType;

                    if (IsFlipped)
                        terrainType = terrainData[((int)Size.X - x - 1) + y * (int)Size.X];
                    else
                        terrainType = terrainData[x + y * (int)Size.X];
                    if (terrainType != 0)
                    {
                        if (beginY == int.MinValue)
                        {
                            beginY = y;
                            beginType = terrainType;
                        }
                    }
                    if (terrainType != beginType)
                    {
                        world.AddLine(new Line
                        {
                            IsSolid = beginType == 1 ? true : false,
                            From = (int)LocalCoord.Y + beginY,
                            To = (int)LocalCoord.Y + y
                        }, (int)LocalCoord.X + x);
                        beginY = int.MinValue;
                        beginType = 0;
                    }
                }
                if (beginY != int.MinValue)
                {
                    world.AddLine(new Line
                    {
                        IsSolid = beginType == 1 ? true : false,
                        From = (int)LocalCoord.Y + beginY,
                        To = (int)LocalCoord.Y + (int)Size.Y
                    }, (int)LocalCoord.X  + x);
                }
            }

            foreach (var enemy in Enemies)
            {
                if (IsFlipped != initialFlipped)
                    Scene.CreateObject<Enemy>(WorldCoord + new Vector2(Size.X, 0f) + new Vector2(-enemy.Center.X, enemy.Center.Y)).Size = enemy.Size;
                else
                    Scene.CreateObject<Enemy>(WorldCoord + new Vector2(enemy.Center.X, enemy.Center.Y)).Size = enemy.Size;
            }
            Enemies.Clear();
        }

        public void Cut()
        {
            initialFlipped = IsFlipped;
            var boundingBox = new Rectangle(WorldCoord, Size);
            
            var enemies = Scene.EnumerateObjects<Enemy>().ToArray();
            foreach (var enemy in enemies)
            {
                if (boundingBox.Intersects(enemy.BoundingBox))
                {
                    var overlay = boundingBox.OverlayOf(enemy.BoundingBox);
                    var miss = overlay.Size - enemy.Size;
                    miss.X = Mathf.Abs(miss.X);
                    miss.Y = Mathf.Abs(miss.Y);
                    
                    if (miss.X > 4f)
                    {
                        if (enemy.LocalCoord.X > boundingBox.Center.X)
                        {
                            Scene.CreateObject<Enemy>(enemy.LocalCoord + new Vector2(enemy.Size.X / 2f, 0f) - new Vector2(miss.X / 2f, 0f)).Size = new Vector2(miss.X, enemy.Size.Y);
                        }
                        else
                        {
                            Scene.CreateObject<Enemy>(enemy.LocalCoord - new Vector2(enemy.Size.X / 2f, 0f) + new Vector2(miss.X / 2f, 0f)).Size = new Vector2(miss.X, enemy.Size.Y);
                        }
                    }
                    if (miss.Y > 4f)
                    {
                        if (enemy.LocalCoord.Y > boundingBox.Center.Y)
                        {
                            Scene.CreateObject<Enemy>(enemy.LocalCoord + new Vector2(0f, enemy.Size.Y / 2f) - new Vector2(0f, miss.Y / 2f)).Size = new Vector2(enemy.Size.X, miss.Y);
                        }
                        else
                        {
                            Scene.CreateObject<Enemy>(enemy.LocalCoord - new Vector2(0f, enemy.Size.Y / 2f) + new Vector2(0f, miss.Y / 2f)).Size = new Vector2(enemy.Size.X, miss.Y);
                        }
                    }

                    if (overlay.Size.X > 2f && overlay.Size.Y > 2f)
                        Enemies.Add(overlay);
                    enemy.Remove();
                }
            }

            terrainData = new int[(int)Size.X * (int)Size.Y];

            var world = ((MainScene)Scene).World;

            for (int x = 0; x < (int)Size.X; x++)
            {
                for (int y = 0; y < (int)Size.Y; y++)
                {
                    if (IsFlipped)
                        terrainData[((int)Size.X - x - 1) + y * (int)Size.X] = world.Sample(LocalCoord + new Vector2(x, y));
                    else
                        terrainData[x + y * (int)Size.X] = world.Sample(LocalCoord + new Vector2(x, y));
                }

                var xx = (int)LocalCoord.X + x;
                if (xx < 0 || xx >= world.Data.Length)
                    continue;
                var list = world.Data[xx];
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    var line = list[i];

                    if (line.IsStatic)
                        continue;
                    if (line.To <= LocalCoord.Y || line.From > LocalCoord.Y + Size.Y)
                        continue;
                    
                    if (line.From >= LocalCoord.Y)
                    {
                        line.From = (int)LocalCoord.Y + (int)Size.Y;
                        if (!line.IsValid)
                        {
                            list.RemoveAt(i);
                            i--;
                            count--;
                        }
                    }
                    else if (line.To < LocalCoord.Y + Size.Y)
                    {
                        line.To = (int)(LocalCoord.Y);
                        if (!line.IsValid)
                        {
                            list.RemoveAt(i);
                            i--;
                            count--;
                        }
                    }
                    else
                    {
                        list.RemoveAt(i);
                        i--;
                        count--;

                        world.AddLine(new Line
                        {
                            IsSolid = line.IsSolid,
                            From = line.From,
                            To = (int)LocalCoord.Y
                        }, (int)LocalCoord.X + x);

                        world.AddLine(new Line
                        {
                            IsSolid = line.IsSolid,
                            From = (int)LocalCoord.Y + (int)Size.Y,
                            To = line.To
                        }, (int)LocalCoord.X + x);
                    }
                }
            }

            for (int x = 0; x < 2; x++)
                for (int y = 0; y < (int)Size.Y; y++)
                    terrainData[x + y * (int)Size.X] = 0;
        }
    }
}
