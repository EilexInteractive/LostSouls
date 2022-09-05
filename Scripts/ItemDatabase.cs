using Godot;
using System;
using System.Collections.Generic;

public enum ItemRarity
{
    Null,
    Common,
    Uncommon,
    Rare,
    Legendary
}

public struct ItemDropData
{
    public Item _ItemDetails;
    public ItemRarity _ItemRarity;

    public ItemDropData(Item item, ItemRarity rare)
    {
        _ItemDetails = item;
        _ItemRarity = rare;
    }
}

public class ItemDatabase : Node
{
    private List<ItemDropData> _Items = new List<ItemDropData>();

    public override void _Ready()
    {
        base._Ready();
        _Items.Add(new ItemDropData(new HealthPotion(25), ItemRarity.Common));
        _Items.Add(new ItemDropData(new HealthPotion(50), ItemRarity.Uncommon));
        _Items.Add(new ItemDropData(new HealthPotion(75), ItemRarity.Rare));
        _Items.Add(new ItemDropData(new Weapon("Old Sword", "Rusty old sword that's been sitting around for a while", 2, null, 1, 1.2f), ItemRarity.Common));
        
    }

    public Item GetItem(string itemName)
    {
        foreach (var item in _Items)
        {
            if (item._ItemDetails.GetItemName() == itemName)
                return item._ItemDetails;
        }

        return null;
    }

    public Item GetItem(string itemName, ItemRarity rarity)
    {
        foreach (var item in _Items)
        {
            if (item._ItemDetails.GetItemName() == itemName && item._ItemRarity == rarity)
            {
                return item._ItemDetails;
            }
        }


        return null;
    }
    
    
}
