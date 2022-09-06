using Godot;
using System;

public class InventoryItem : Button
{
    private Item _OwningItem;                   // Reference to the item that's being displayed

    public void Setup(Item item) => _OwningItem = item;

    public void OnPressed()
    {
        _OwningItem.UseItem();                  // Use the item
        
        // If it's a consumable make sure to remove it from the inventory
        if (_OwningItem.GetItemType() == ItemType.Consumable)
        {
            GetNode<InventoryUI>("/root/Main/Player/Canvas/InventoryRect").RemoveItemButton(this);
        }
    }
    
}
