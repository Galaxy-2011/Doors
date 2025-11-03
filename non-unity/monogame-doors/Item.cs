using Microsoft.Xna.Framework;

namespace MonogameDoors
{
    public class Item
    {
        public string Name { get; set; }
        public Vector2 Position { get; set; }

        public Item(string name, Vector2 pos)
        {
            Name = name;
            Position = pos;
        }
    }
}
