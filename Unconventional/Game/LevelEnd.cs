using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cog.Modules.Content;
using Cog.Modules.EventHost;
using Cog.Modules.Renderer;
using Cog;
using System.IO;

namespace Unconventional.Game
{
    class LevelEnd : ParticleSystem
    {
        public Texture Texture;
        private float cd;
        
        public LevelEnd()
            : base(32)
        {
            Texture = Program.Pixel;
            
            RegisterEvent<PhysicsUpdateEvent>(0, PhysicsUpdate);
            var back = SpriteComponent.RegisterOn(this, Program.Portal);
            back.Color = Program.Foreground;
            back.Scale = new Vector2(1.1f, 1.1f);
            SpriteComponent.RegisterOn(this, Program.Portal);
        }

        public void PhysicsUpdate(PhysicsUpdateEvent ev)
        {
            if (cd > 0f)
            {
                cd -= ev.DeltaTime;
                if (cd < 0f)
                    cd = 0f;
            }
            while (ParticleCount < 32 && cd == 0f && TimeSinceDraw.Elapsed.TotalSeconds < 0.5f)
            {
                var angle = new Angle(Engine.RandomFloat() * 360f);
                Add(new Particle
                {
                    Life = 60f,
                    Scale = new Vector2(4f, 4f),
                    Coordinate = WorldCoord + angle.Unit * 20f,
                    Speed = (angle + new Angle(90f)).Unit * 300f,
                    Color = Program.Foreground
                });
                cd = Engine.RandomFloat() * 0.1f;
            }

            var player = ((MainScene)Scene).Player;
            if (player != null)
            {
                var distance = (WorldCoord - player.WorldCoord).Length;
                if (distance < 160)
                {
                    if (distance < 30)
                    {
                        player.WorldScale *= Mathf.Max(0f, 1f - ev.DeltaTime * 5f);
                        player.Speed = Vector2.Zero;

                        /*Scene.Camera.WorldScale += new Vector2(0.1f, 0.1f) * ev.DeltaTime;
                        Scene.Camera.WorldRotation -= Angle.FromDegree(90f * ev.DeltaTime);
                        Scene.Camera.WorldCoord = player.WorldCoord;*/

                        if (player.WorldScale.X <= 0.01f)
                        {
                            World.LevelNum++;
                            Engine.SceneHost.Pop();
                            if (Engine.ResourceHost.GetContainer("main").ReadData(string.Format("level_{0}.png", World.LevelNum)) == null)
                                Engine.SceneHost.Push(Engine.SceneHost.CreateGlobal<EndScene>());
                            else
                                Engine.SceneHost.Push(Engine.SceneHost.CreateGlobal<MainScene>());
                        }
                    }
                    else
                        player.Speed += (WorldCoord - player.WorldCoord).Unit * 3000f * ev.DeltaTime;
                    player.LocalRotation += new Angle(360f * ev.DeltaTime);
                    player.Stun = 1f;
                }
            }
        }

        public override void UpdateAndDraw(Particle particle, float deltaTime, IRenderTarget target)
        {
            particle.Speed += (LocalCoord - particle.Coordinate).Unit * 800f * deltaTime;
            particle.Rotation += Angle.FromDegree(180f * deltaTime);
            particle.Coordinate += particle.Speed * deltaTime;
            if ((LocalCoord - particle.Coordinate).Length < 15f)
            {
                particle.Life = 0f;
            }

            target.DrawTexture(Texture, particle.Coordinate, particle.Color, particle.Scale, Vector2.One * .5f, particle.Rotation.Degree, new Rectangle(Vector2.Zero, Texture.Size));
        }
    }
}
