using Godot;
using System;

public enum ItemType
{
    Weapon,
    Consumable,
    Armour
}

[Serializable]
public abstract class Item
{
    // === ITEM DETAILS === //
    protected string _ItemName;
    protected string _ItemDescription;
    protected int _ItemCost;
    protected ItemType _ItemType;

    protected Character _Owner;
    protected ItemRarity _Rarity;

    public Item(string name, string desc, int cost, ItemType type, ItemRarity rare = ItemRarity.Common)
    {
        _ItemName = name;
        _ItemDescription = desc;
        _ItemCost = cost;
        _ItemType = type;
        _Rarity = rare;
    }

    
 
    /// <summary>
    /// Uses the item
    /// </summary>
    public abstract void UseItem();

    
    // === GETTERS === //
    public string GetItemName() => _ItemName;
    public string GetItemDesc() => _ItemDescription;
    public int GetItemCost() => _ItemCost;
    public ItemType GetItemType() => _ItemType;
    public Character GetOwner() => _Owner;
    
    // === SETTERS === //
    public void SetOwner(Character owner) => _Owner = owner;

}