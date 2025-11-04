using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class JackEntity : Entity
    {
        private float cooldown = 0f;
        private float speed = 80f;

        public JackEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (IsDead) return;

            cooldown -= dt;
            var dir = player.Position - Position;
            float dist = dir.Length();

            // occasional knock/appearance when player is near
            if (dist < 120f && cooldown <= 0f && !player.IsHidden)
            {
                VFXManager.AddTrauma(0.4f);
                AudioManager.Play("jack_knock", 0.9f);
                player.Sanity = MathHelper.Clamp(player.Sanity - 20f, 0f, 100f);
                cooldown = 4f;
            }

            // chase behavior for close-range attack
            if (dist > 1e-3f)
            {
                dir.Normalize();
                Position += dir * speed * dt;
            }

            if (!player.IsHidden && dist < 26f)
            {
                player.Sanity = MathHelper.Clamp(player.Sanity - 30f, 0f, 100f);
                VFXManager.AddTrauma(0.9f);
                AudioManager.Play("jack_bite", 1f);
                IsDead = true;
            }
        }
    }
}
