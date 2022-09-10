using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Text;

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
        // Health Potions
        _Items.Add(new ItemDropData(new HealthPotion(25), ItemRarity.Common));
        _Items.Add(new ItemDropData(new HealthPotion(50), ItemRarity.Uncommon));
        _Items.Add(new ItemDropData(new HealthPotion(75), ItemRarity.Rare));
        _Items.Add(new ItemDropData(new HealthPotion(100), ItemRarity.Legendary));
        
        // Common Weapons
        _Items.Add(new ItemDropData(new Weapon("Old Sword", "Rusty old sword that's been sitting around for a while", 2, WeaponType.LONG_SWORD, null, 1, 1.2f), ItemRarity.Common));
        _Items.Add(new ItemDropData(new Weapon("Norse Axe", "A steel axe commonly used by great viking warriors", 5, WeaponType.AXE, null, 1.2f, 1.4f), ItemRarity.Common));
        _Items.Add(new ItemDropData(new Weapon("Sword of Strength", "A heavy sword used by only the strongest of the dungeon", 7, WeaponType.LONG_SWORD, null, 1.5f, 1.8f), ItemRarity.Common));
        
        // Common Armour
        _Items.Add(new ItemDropData(new Armour("Light Armour", "Common light weight armour used by training warriors", 3, ItemType.Armour), ItemRarity.Common));

        // Uncommon Weapons
        _Items.Add(new ItemDropData(new Weapon("Sword of Knights", "This sword was used by only the greatest of knights", 15, WeaponType.LONG_SWORD, null, 1.7f, 2.0f), ItemRarity.Uncommon));
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

    public Item GetRandomItem(ItemRarity rare)
    {
        List<Item> PotenialItems = new List<Item>();
        
        switch (rare)
        {
            case ItemRarity.Common:
                PotenialItems = GetCommonItems();
                break;
            case ItemRarity.Uncommon:
                PotenialItems = GetUncommonItems();
                break;
            case ItemRarity.Rare:
                PotenialItems = GetRareItems();
                break;
            case ItemRarity.Legendary:
                PotenialItems = GetLegendItems();
                break;
        }

        float randValue = (float)GD.RandRange(0, PotenialItems.Count - 1);

        return PotenialItems[(int)randValue];   
    }

    public List<Item> GetCommonItems()
    {
        List<Item> common = new List<Item>();
        foreach(var item in _Items)
            if(item._ItemRarity == ItemRarity.Common)
                common.Add(item._ItemDetails);

        return common;
    }

    public List<Item> GetUncommonItems()
    {
        List<Item> uncommon = new List<Item>();
        foreach(var item in _Items)
            if(item._ItemRarity == ItemRarity.Uncommon)
                uncommon.Add(item._ItemDetails);

        return uncommon;
    }

    public List<Item> GetRareItems()
    {
        List<Item> rare = new List<Item>();
        foreach(var item in _Items)
            if(item._ItemRarity == ItemRarity.Rare)
                rare.Add(item._ItemDetails);

        return rare;
    }

    public List<Item> GetLegendItems()
    {
        List<Item> legend = new List<Item>();
        foreach(var item in _Items)
            if(item._ItemRarity == ItemRarity.Legendary)
                legend.Add(item._ItemDetails);

        return legend;
    }
    
    
}
