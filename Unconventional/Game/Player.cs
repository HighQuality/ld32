using Cog;
using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.EventHost;
using Cog.Modules.Content;
using Cog.Modules.Renderer;

namespace Unconventional.Game
{
    class Player : PhysicsObject
    {
        private float runTime;
        private SpriteComponent sprite;
        private Texture texture;
        public bool Enabled = false;
        private Angle rotation;
        private bool oldAction;
        private Snapshot currentSnap;

        private Vector2 textureSize = new Vector2(40f, 64f);
        public float Stun = 0f;

        public Player()
            : base(new Cog.Vector2(24f, 64f))
        {
            texture = Program.Player;
            sprite = SpriteComponent.RegisterOn(this, texture);
            sprite.Origin = new Vector2(textureSize.X / 2f, textureSize.Y / 2f);
            sprite.Color = Program.Foreground;

            RegisterEvent<KeyDownEvent>((int)Keyboard.Key.R, 0, Restart);

            Depth = -1;
        }

        public void Restart(KeyDownEvent ev)
        {
            Engine.SceneHost.Pop();
            Engine.SceneHost.Push(Engine.SceneHost.CreateGlobal<MainScene>());
            ev.Intercept = true;
        }

        public int DistanceToGround(int relX, int min, int max)
        {
            for (int i=min; i<max; i++)
            {
                if (!IsFree(new Vector2(relX, i)))
                    return i;
                i++;
            }
            return max;
        }

        public override void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            Stun -= ev.DeltaTime;
            if (Stun < 0f)
            {
                Stun = 0f;
            }

            bool left = Engine.Window.IsKeyDown(Keyboard.Key.Left) || Engine.Window.IsKeyDown(Keyboard.Key.A),
                right = Engine.Window.IsKeyDown(Keyboard.Key.Right) || Engine.Window.IsKeyDown(Keyboard.Key.D),
                up = Engine.Window.IsKeyDown(Keyboard.Key.Up) || Engine.Window.IsKeyDown(Keyboard.Key.W),
                action = Engine.Window.IsKeyDown(Keyboard.Key.Space) ||
                Engine.Window.IsKeyDown(Keyboard.Key.X) ||
                Engine.Window.IsKeyDown(Keyboard.Key.V) ||
                Engine.Window.IsKeyDown(Keyboard.Key.Z);

            if (!Enabled || Stun > 0f)
            {
                left = false;
                right = false;
                up = false;
                action = false;
            }

            if (IsOnGround)
            {
                if (left && !right)
                {
                    Speed.X -= 2400f * ev.DeltaTime;
                    LocalScale = new Vector2(-1f, 1f);
                }

                if (right && !left)
                {
                    Speed.X += 2400f * ev.DeltaTime;
                    LocalScale = new Vector2(1f, 1f);
                }

                if (action && !oldAction)
                {
                    if (currentSnap != null)
                    {
                        currentSnap.LocalCoord = WorldCoord + new Vector2(Size.X / 2f, Size.Y / 2f);
                        currentSnap.LocalCoord -= new Vector2(0f, currentSnap.Size.Y);
                        currentSnap.IsFlipped = LocalScale.X < 0f;
                        if (currentSnap.IsFlipped)
                            currentSnap.LocalCoord -= new Vector2(Size.X + currentSnap.Size.X, 0f);
                    }

                    Program.Cut.Play();
                    var snap = Scene.CreateObject<Snapshot>(null, Vector2.Zero, currentSnap, this);
                    currentSnap = snap;
                    currentSnap.LocalCoord = WorldCoord + new Vector2(Size.X / 2f, Size.Y / 2f);
                    currentSnap.LocalCoord -= new Vector2(0f, currentSnap.Size.Y);
                    currentSnap.IsFlipped = LocalScale.X < 0f;
                    if (currentSnap.IsFlipped)
                        currentSnap.LocalCoord -= new Vector2(Size.X + currentSnap.Size.X, 0f);
                }
            }
            else
            {
                if (left && !right)
                    Speed.X -= 240f * ev.DeltaTime;
                if (right && !left)
                    Speed.X += 240f * ev.DeltaTime;

                if (WorldCoord.Y - 200 > World.SolidsHeight)
                {
                    Engine.SceneHost.Pop();
                    Engine.SceneHost.Push(Engine.SceneHost.CreateGlobal<MainScene>());
                }
            }

            Speed.X = Mathf.Min(Mathf.Abs(Speed.X), 240f) * (Speed.X < 0f ? -1f : 1f);

            if ((left || right) && !(right && left))
                runTime += ev.DeltaTime;
            else
                runTime = 0f;

            if (!IsOnGround)
            {
                sprite.TextureRect = new Rectangle(new Vector2(textureSize.X, 0f), textureSize);
            }
            else if (runTime == 0f)
            {
                sprite.TextureRect = new Rectangle(Vector2.Zero, textureSize);
            }
            else
            {
                sprite.TextureRect = new Rectangle(new Vector2((2 + ((int)(runTime * 12f) % 6)) * textureSize.X, 0f), textureSize);
            }

            if (up && IsOnGround)
            {
                Speed.Y = -400f;
            }

            // Leaning
            if (Stun == 0f)
            {
                Angle angle;
                if (IsOnGround)
                {
                    var leftDist = DistanceToGround(-5, -2, 4);
                    var rightDist = DistanceToGround(5, -2, 4);
                    angle = Angle.FromVector(new Vector2(5f, rightDist) - new Vector2(-5f, leftDist));
                }
                else
                {
                    angle = new Angle(0f);
                }
                var deltaAngle = ((((angle.Degree - rotation.Degree) % 360f) + 540f) % 360f) - 180f;
                rotation += new Angle(deltaAngle * Mathf.Min(1f, ev.DeltaTime * 10f));
                if (LocalScale.X < 0f)
                    LocalRotation = -rotation;
                else
                    LocalRotation = rotation;
            }

            oldAction = action;

            base.PhysicsUpdate(ev);
        }
    }
}
