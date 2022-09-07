using Godot;
using System;

public class InventoryItem : Button
{
    private Item _OwningItem;                   // Reference to the item that's being displayed

    public void Setup(Item item)
    {
        _OwningItem = item;
    } 

    public void OnPressed()
    {
        _OwningItem.UseItem();                  // Use the item
        
        
        if (_OwningItem.GetItemType() == ItemType.Consumable)
        {
            // If it's a consumable make sure to remove it from the inventory
            GetNode<InventoryUI>("/root/Main/Player/Canvas/InventoryRect").RemoveItemButton(this);
        } else if (_OwningItem.GetItemType() == ItemType.Weapon)
        {
            // If it's a weapon than equip the weapon & update the inventory
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().EquipWeapon(_OwningItem as Weapon);
            GetNode<InventoryUI>("/root/Main/Player/Canvas/InventoryRect").SetEquippedWeapon();
        }
    }

    public Item GetItem() => _OwningItem;

}
