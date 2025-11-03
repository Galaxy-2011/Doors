using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class ScreechEntity : Entity
    {
        private float appearRange = 160f;
        private float reactionWindow = 1f;
        private bool active = false;
        private float timer = 0f;

        public ScreechEntity(Vector2 pos) : base(pos) { }

        public override void Update(float dt, Player player)
        {
            if (active)
            {
                timer += dt;
                // If player presses Space within reactionWindow, they avoid damage
                var ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                {
                    // Player reacted
                    VFXManager.AddTrauma(0.15f);
                    AudioManager.Play("screech_miss");
                    IsDead = true; // disappears
                    return;
                }

                if (timer > reactionWindow)
                {
                    // failed to react
                    player.Sanity -= 30f;
                    VFXManager.AddTrauma(0.6f);
                    AudioManager.Play("screech_hit");
                    IsDead = true;
                }
                return;
            }

            float d = Vector2.Distance(Position, player.Position);
            if (d < appearRange && !player.IsHidden)
            {
                // Appear and start reaction window
                active = true;
                timer = 0f;
                AudioManager.Play("screech_appear");
            }
        }
    }
}
