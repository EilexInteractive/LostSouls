using Godot;
using System;
using System.Collections.Generic;

[Serializable]
public class InventoryComponent
{
    private Character _OwningCharacter;                         // Reference to the owning character

    private List<Item> _Inventory = new List<Item>();               // List of items in inventory
    private int _InventorySize = 5;                                     // How many items can be stored in the inventory

    protected Weapon _EquippedWeapon;                       // Weapon that the character currently is using
    // TODO: Add Armour Here

    public InventoryComponent(Character owner)
    {
        _OwningCharacter = owner;
    }

    /// <summary>
    /// Adds a new item to the inventory if there is room
    /// </summary>
    /// <param name="item">Items to add</param>
    public bool AddItem(Item item)
    {
        if (_Inventory.Count < _InventorySize && item != null)
        {
            _Inventory.Add(item);
            item.SetOwner(_OwningCharacter);
            return true;
        }

        return false;
    }

    
    /// <summary>
    /// Removes the specific item from the inventory if it exist
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        if (item != null && _Inventory.Contains(item))
            _Inventory.Remove(item);
    }

    public void PrintInventory()
    {
        foreach (var item in _Inventory)
        {
            GD.Print(item.GetItemName());
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (_Inventory.Contains(weapon))
        {
            _EquippedWeapon = weapon;
            var pc = _OwningCharacter.GetController() as PlayerController;
            pc?.SetEquippedItem(weapon);
        }
    }


    public Weapon GetEquippedWeapon() => _EquippedWeapon;
    public List<Item> GetInventoryItems() => _Inventory;
}