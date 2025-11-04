using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class TimothyEntity : Entity
    {
        private float speed = 30f;
        private float tick = 0f;

        public TimothyEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (IsDead) return;

            // slowly follow the player
            var dir = player.Position - Position;
            float dist = dir.Length();
            if (dist > 1e-3f)
            {
                dir.Normalize();
                Position += dir * speed * dt;
            }

            tick += dt;
            // Emits a low hum that lowers sanity gradually when nearby
            if (dist < 140f)
            {
                if (tick > 0.8f)
                {
                    player.Sanity = MathHelper.Clamp(player.Sanity - 4f, 0f, 100f);
                    AudioManager.Play("timothy_hum", 0.45f);
                    VFXManager.AddTrauma(0.08f);
                    tick = 0f;
                }
            }
        }
    }
}
