using Godot;
using System;

public class InventoryItem : Button
{
    private Item _OwningItem;                   // Reference to the item that's being displayed
    private InventoryUI _InvUI;                        // Reference to the inventory UI

    public void Setup(Item item, InventoryUI ui)
    {
        _OwningItem = item;
        _InvUI = ui;
    } 

    public void OnPressed()
    {
        _InvUI.SetViewingItem(_OwningItem, this);
    }

    public Item GetItem() => _OwningItem;

}
