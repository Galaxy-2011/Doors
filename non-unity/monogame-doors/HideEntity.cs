using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class HideEntity : Entity
    {
        private float speed = 80f;

        public HideEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            // Move toward player slowly; if player is hidden, do nothing
            if (player.IsHidden) return;

            var dir = (player.Position - Position);
            if (dir.LengthSquared() > 0) dir.Normalize();
            Position += dir * speed * dt;

            if (Vector2.Distance(Position, player.Position) < 16)
            {
                // instant kill
                player.Sanity = 0;
                IsDead = true;
            }
        }
    }
}
