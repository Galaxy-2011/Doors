using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class SaveData
    {
        public int CurrentRoom { get; set; }
        public Vector2 PlayerPosition { get; set; }
        public float PlayerSanity { get; set; }
    }
}
