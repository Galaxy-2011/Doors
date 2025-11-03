using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public abstract class Entity
    {
        public Vector2 Position;
        public bool IsDead = false;

        public Entity(Vector2 pos)
        {
            Position = pos;
        }

        public abstract void Update(float dt, Player player);
    }
}
