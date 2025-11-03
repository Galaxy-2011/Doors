using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class RushEntity : Entity
    {
        private float speed = 300f;
        private float cooldown = 4f;
        private float timer = 0f;
        private bool isRushing = false;

        public RushEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            timer += dt;
            float dist = Vector2.Distance(Position, player.Position);
            if (!isRushing)
            {
                if (dist < 200 && timer > cooldown)
                {
                    isRushing = true;
                    timer = 0f;
                }
            }

            if (isRushing)
            {
                var dir = (player.Position - Position);
                if (dir.LengthSquared() > 0) dir.Normalize();
                Position += dir * speed * dt;

                if (Vector2.Distance(Position, player.Position) < 18)
                {
                    // kill player (reduce sanity to 0)
                    player.Sanity = 0;
                    IsDead = true; // remove entity after
                }

                // stop rushing after some time
                if (timer > 2f) { isRushing = false; timer = 0f; }
            }
            else
            {
                // idle wandering
                Position.X += (float)System.Math.Sin(timer * 0.5f) * 10f * dt;
            }
        }
    }
}
