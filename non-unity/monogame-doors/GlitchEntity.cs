using Microsoft.Xna.Framework;
using System;

namespace MonogameDoors
{
    public class GlitchEntity : Entity
    {
        private float timer = 0f;
        private Random rnd = new Random();

        public GlitchEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (IsDead) return;
            timer -= dt;
            if (timer <= 0f)
            {
                // teleport to a nearby random offset
                float ox = (float)(rnd.NextDouble() * 200 - 100);
                float oy = (float)(rnd.NextDouble() * 120 - 60);
                Position = new Vector2(MathHelper.Clamp(Position.X + ox, 0, 2000), MathHelper.Clamp(Position.Y + oy, 0, 2000));
                VFXManager.AddTrauma(0.25f);
                AudioManager.Play("glitch_pop", 0.6f);
                timer = 0.6f + (float)rnd.NextDouble() * 1.2f;
            }

            // if close to player, quick sanity hit
            if (Vector2.Distance(player.Position, Position) < 28f && !player.IsHidden)
            {
                player.Sanity = MathHelper.Clamp(player.Sanity - 15f, 0f, 100f);
                AudioManager.Play("glitch_bite", 0.8f);
                IsDead = true;
            }
        }
    }
}
