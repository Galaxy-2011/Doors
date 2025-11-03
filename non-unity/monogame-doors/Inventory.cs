using System.Collections.Generic;

namespace MonogameDoors
{
    public class Inventory
    {
        private List<Item> items = new List<Item>();

        public void Add(Item item)
        {
            items.Add(item);
            System.Console.WriteLine($"Picked up {item.Name}");
        }

        public bool Has(string name)
        {
            return items.Exists(i => i.Name == name);
        }

        public void Remove(string name)
        {
            var it = items.Find(i => i.Name == name);
            if (it != null) items.Remove(it);
        }
    }
}
