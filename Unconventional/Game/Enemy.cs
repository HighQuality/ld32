using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Content;
using Cog;

namespace Unconventional.Game
{
    class Enemy : PhysicsObject
    {
        private float timeSinceJump = 0f;
        private bool activated = false;
        private SpriteComponent sc;

        public Enemy()
            : base(new Cog.Vector2(32f, 32f))
        {
            timeSinceJump = Engine.RandomFloat() * -.1f;

            sc = SpriteComponent.RegisterOn(this, Program.Pixel);
            sc.Origin = Vector2.One / 2f;
            sc.Color = Program.Foreground;
        }

        public override void PhysicsUpdate(Cog.Modules.EventHost.PhysicsUpdateEvent ev)
        {
            sc.Scale = Size;
            var player = ((MainScene)Scene).Player;
            if (!player.DoRemove)
            {
                if (player.BoundingBox.Intersects(BoundingBox))
                {
                    player.Remove();

                    Program.EnemyHit.Play();
                    Engine.InvokeTimed(1f, offset =>
                    {
                        Engine.SceneHost.Pop();
                        Engine.SceneHost.Push(Engine.SceneHost.CreateGlobal<MainScene>());
                    });
                }

            }
            else
                player = null;

            if (player != null && player.Enabled)
            {
                if (!activated)
                {
                    if ((WorldCoord - player.WorldCoord).Length < 400f)
                    {
                        activated = true;
                    }
                }
                else
                {
                    timeSinceJump += ev.DeltaTime;

                    if (timeSinceJump >= 1f)
                    {
                        if (IsOnGround)
                        {
                            if (player.WorldCoord.X < WorldCoord.X)
                            {
                                Speed.X = -200f;
                                Speed.Y = -300f;
                            }
                            else
                            {
                                Speed.X = 200f;
                                Speed.Y = -300f;
                            }
                            timeSinceJump = 0f;
                        }
                    }
                }

                if (!IsOnGround)
                {
                    if (Mathf.Abs(player.WorldCoord.X - WorldCoord.X) < 16f)
                    {
                        Speed.X *= Mathf.Max(0f, 1f - ev.DeltaTime * 20f);
                    }
                }
            }

            base.PhysicsUpdate(ev);
        }
    }
}
