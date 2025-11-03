using UnityEngine;
using System.Collections.Generic;

public abstract class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite icon;
    public bool isConsumable;
    public bool isEquippable;
    
    public abstract void Use();
    public virtual void OnEquip() { }
    public virtual void OnUnequip() { }
}

public class Inventory : MonoBehaviour
{
    public int maxItems = 6;
    private List<Item> items = new List<Item>();
    private Item equippedItem;

    public bool AddItem(Item item)
    {
        if (items.Count >= maxItems)
            return false;

        items.Add(item);
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (equippedItem == item)
            UnequipItem();
            
        items.Remove(item);
    }

    public void UseItem(Item item)
    {
        if (items.Contains(item))
        {
            item.Use();
            if (item.isConsumable)
                RemoveItem(item);
        }
    }

    public void EquipItem(Item item)
    {
        if (!items.Contains(item) || !item.isEquippable)
            return;

        if (equippedItem != null)
            UnequipItem();

        equippedItem = item;
        item.OnEquip();
    }

    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            equippedItem.OnUnequip();
            equippedItem = null;
        }
    }

    public List<Item> GetItems()
    {
        return new List<Item>(items);
    }

    public Item GetEquippedItem()
    {
        return equippedItem;
    }
}