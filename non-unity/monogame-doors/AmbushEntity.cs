using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class AmbushEntity : Entity
    {
        private float waitTime = 1.2f;
        private float timer = 0f;
        private bool ambushing = false;
        private Vector2 startPos;
        private float rushSpeed = 360f;

        public AmbushEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (!ambushing)
            {
                float d = Vector2.Distance(Position, player.Position);
                if (d < 180 && !player.IsHidden)
                {
                    ambushing = true;
                    timer = 0f;
                    startPos = Position;
                    AudioManager.Play("ambush_warn");
                }
            }
            else
            {
                timer += dt;
                if (timer < waitTime) return;

                // Rush toward player
                var dir = (player.Position - Position);
                if (dir.LengthSquared() > 0) dir.Normalize();
                Position += dir * rushSpeed * dt;

                if (Vector2.Distance(Position, player.Position) < 18)
                {
                    // attack
                    player.Sanity -= 50f;
                    VFXManager.AddTrauma(0.8f);
                    AudioManager.Play("ambush_hit");
                    IsDead = true;
                }

                // if chased too long, vanish
                if (Vector2.Distance(Position, startPos) > 800)
                {
                    IsDead = true;
                }
            }
        }
    }
}
