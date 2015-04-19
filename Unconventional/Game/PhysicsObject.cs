using Cog;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unconventional.Game
{
    abstract class PhysicsObject : GameObject
    {
        public Vector2 Speed;

        private Vector2 relPosAcc;

        public float Gravity = 900f;

        public float GroundFriction = 1000f,
            AirFriction = 0f;

        public bool IsOnGround { get; private set; }

        public World World;

        public DustSystem dustSystem;

        public PhysicsObject(Vector2 size)
        {
            RegisterEvent<PhysicsUpdateEvent>(0, PhysicsUpdate);
            this.Size = size;
        }

        public override void Initialize()
        {
            dustSystem = Scene.CreateLocalObject<DustSystem>(this, Vector2.Zero);
            
            base.Initialize();
        }

        public virtual void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            World = ((MainScene)Scene).World;
            Speed.Y += Gravity * ev.DeltaTime;

            relPosAcc += Speed * ev.DeltaTime;

            while (Mathf.Abs(relPosAcc.X) >= 1f)
            {
                int sign = relPosAcc.X > 0f ? 1 : -1;
                if (IsFree(new Vector2(sign, 0f)))
                {
                    if (!IsFree(new Vector2(0f, 1f)) && IsFree(new Vector2(sign, 1f)))
                    {
                        if (!IsFree(new Vector2(sign, 2f)))
                            LocalCoord += new Vector2(0f, 1f);
                        else if (!IsFree(new Vector2(sign, 3f)))
                            LocalCoord += new Vector2(0f, 2f);
                    }
                    LocalCoord += new Vector2(sign, 0f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -1f)))
                {
                    LocalCoord += new Vector2(sign, -1f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -2f)))
                {
                    LocalCoord += new Vector2(sign, -2f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -3f)))
                {
                    LocalCoord += new Vector2(sign, -3f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -4f)))
                {
                    LocalCoord += new Vector2(sign, -4f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -5f)))
                {
                    LocalCoord += new Vector2(sign, -5f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -6f)))
                {
                    LocalCoord += new Vector2(sign, -6f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -7f)))
                {
                    LocalCoord += new Vector2(sign, -7f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -8f)))
                {
                    LocalCoord += new Vector2(sign, -8f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -9f)))
                {
                    LocalCoord += new Vector2(sign, -9f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -10f)))
                {
                    LocalCoord += new Vector2(sign, -10f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -11f)))
                {
                    LocalCoord += new Vector2(sign, -11f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -12f)))
                {
                    LocalCoord += new Vector2(sign, -12f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -13f)))
                {
                    LocalCoord += new Vector2(sign, -13f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -14f)))
                {
                    LocalCoord += new Vector2(sign, -14f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -15f)))
                {
                    LocalCoord += new Vector2(sign, -15f);
                    relPosAcc.X -= sign;
                }
                else if (IsFree(new Vector2(sign, -16f)))
                {
                    LocalCoord += new Vector2(sign, -16f);
                    relPosAcc.X -= sign;
                }
                else
                {
                    relPosAcc.X = 0f;
                    Speed.X = 0f;
                    break;
                }
            }
            while (Mathf.Abs(relPosAcc.Y) >= 1f)
            {
                int sign = relPosAcc.Y > 0f ? 1 : -1;
                if (IsFree(new Vector2(0f, sign)))
                {
                    LocalCoord += new Vector2(0f, sign);
                    relPosAcc.Y -= sign;
                }
                else
                {
                    if (Speed.Y >= 200f)
                    {
                        const int count = 10;
                        // Program.Ground.Play();

                        for (int i = 0; i < count; i++)
                        {
                            dustSystem.Add(new Particle
                            {
                                Coordinate = WorldCoord + new Vector2(-Size.X / 2f + ((float)i / ((float)count - 1f)) * Size.X * .5f, Size.Y / 2f),
                                Scale = new Vector2(4f, 4f),
                                Speed = new Vector2(-1f, 0f).Rotate(Angle.FromDegree(45f + Engine.RandomFloat() * 90f)) * 200f,
                                Life = 1f + Engine.RandomFloat() * 0.1f,
                            });
                        }
                    }

                    relPosAcc.Y = 0f;
                    Speed.Y = 0f;
                    break;
                }
            }

            ReEvaluateIsOnGround();

            if (IsOnGround)
                Speed.X = Mathf.Max(0f, Mathf.Abs(Speed.X) - GroundFriction * ev.DeltaTime) * (Speed.X < 0f ? -1f : 1f);
            else
                Speed.X = Mathf.Max(0f, Mathf.Abs(Speed.X) - AirFriction * ev.DeltaTime) * (Speed.X < 0f ? -1f : 1f);
        }
        
        public void ReEvaluateIsOnGround()
        {
            IsOnGround = !IsFree(new Vector2(0f, 1f));
        }

        public bool IsFree(Vector2 relativePosition)
        {
            return World.IsFree(new Rectangle(LocalCoord - Size / 2f + relativePosition, Size));
        }
    }
}
