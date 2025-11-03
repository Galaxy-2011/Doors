using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class FigureEntity : Entity
    {
        private float speed = 30f;

        public FigureEntity(Vector2 pos) : base(pos)
        {
        }

        public override void Update(float dt, Player player)
        {
            if (IsDead) return;

            // Simple approach: slowly approach the player
            var dir = player.Position - Position;
            float dist = dir.Length();
            if (dist > 1e-3f)
            {
                dir.Normalize();
                Position += dir * speed * dt;
            }

            // If close and player is not hidden, do a jump-scare
            if (dist < 30f && !player.IsHidden)
            {
                // apply heavy sanity hit and strong camera trauma
                player.Sanity = MathHelper.Clamp(player.Sanity - 40f, 0f, 100f);
                VFXManager.AddTrauma(1f);
                AudioManager.Play("figure_scream", 1f);
                IsDead = true;
            }
        }
    }
}
