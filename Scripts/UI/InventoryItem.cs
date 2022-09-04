using Godot;
using System;

public class InventoryItem : Button
{
    private Item _OwningItem;

    public void Setup(Item item) => _OwningItem = item;

    public void OnPressed()
    {
        _OwningItem.UseItem();
        if (_OwningItem.GetItemType() == ItemType.Consumable)
        {
            GetNode<InventoryUI>("/root/Main/Player/Canvas/InventoryRect").RemoveItemButton(this);
        }
    }
    
}
