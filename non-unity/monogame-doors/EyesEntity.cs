using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class EyesEntity : Entity
    {
        private float detectionRadius = 150f;
        private float cooldown = 0f;

        public EyesEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (IsDead) return;

            cooldown -= dt;
            var toPlayer = player.Position - Position;
            float dist = toPlayer.Length();

            if (dist < detectionRadius && cooldown <= 0f)
            {
                var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                bool playerMoving = ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D);

                if (!player.IsHidden && !playerMoving)
                {
                    // teleport to a flank position and punish sanity
                    Position = player.Position + new Vector2(-40 + (float)(System.Random.Shared.NextDouble() * 80), -20);
                    VFXManager.AddTrauma(0.6f);
                    AudioManager.Play("eyes_stare", 0.85f);
                    player.Sanity = MathHelper.Clamp(player.Sanity - 20f, 0f, 100f);
                    cooldown = 3f;
                }
            }
        }
    }
}
