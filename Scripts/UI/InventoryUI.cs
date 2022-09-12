using Godot;
using System;
using System.Collections.Generic;


public class InventoryUI : ColorRect
{
    [Export] private PackedScene _ItemButton;           // Reference to the button that will spawn with it details

    private bool _CanOpen = true;
    private bool _CanClose = false;

    private Item _ViewingItem;
    private Button _ViewingBtn;

    private List<Button> _SpawnedButtons = new List<Button>();

    public override void _Process(float delta)
    {
        base._Process(delta);
        
    }


    public void OpenInventory(InventoryComponent inventory)
    {
        this.Show();                    // Show the inventory

        foreach (var item in inventory.GetInventoryItems())
        {
            // Create the instance and spawn to the scene
            var btnInstance = _ItemButton.Instance();
            GetNode<VBoxContainer>("VBoxContainer").AddChild(btnInstance);
            
            // Cast to a button and set the text of the button
            // to the name of the item
            var btnNode = btnInstance as Button;
            if(btnNode != null)
                btnNode.Text = item.GetItemName();

            var itemButton = btnNode as InventoryItem;
            if (itemButton != null)
            {
                itemButton.Setup(item, this);
                SetEquippedWeapon();
            }
            
            SetEquippedWeapon();
               
            
            _SpawnedButtons.Add(btnNode);
        }
        
    }

    public void SetViewingItem(Item item, Button itemBtn)
    {
        GetNode<ColorRect>("ViewingItem").Show();
        _ViewingItem = item;
        _ViewingBtn = itemBtn;

        
    }

    private string GetPathToIcon(Item item)
    {
        var iconPath = "res://Sprites/Icon/";
        
    }

    public void SetEquippedWeapon()
    {
        Item equippedItem = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().GetEquippedWeapon();
        foreach (var btn in _SpawnedButtons)
        {
            
            var itemInstance = btn as InventoryItem;
            btn.Text = itemInstance?.GetItem()?.GetItemName();
            if (itemInstance.GetItem() == equippedItem)
                btn.Text += "*";
        }
    }

    public void OnActionPressed()
    {
        _ViewingItem.UseItem();                  // Use the item
        
        
        if (_ViewingItem.GetItemType() == ItemType.Consumable)
        {
            // If it's a consumable make sure to remove it from the inventory
            GetNode<InventoryUI>("/root/Main/Player/Canvas/CanvasLayer/InventoryRect").RemoveItemButton(_ViewingBtn));
        } else if (_ViewingItem.GetItemType() == ItemType.Weapon)
        {
            // If it's a weapon than equip the weapon & update the inventory
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().EquipWeapon(_ViewingItem as Weapon);
            GetNode<InventoryUI>("/root/Main/Player/Canvas/CanvasLayer/InventoryRect").SetEquippedWeapon();
        }
    }

    public void OnDropPressed()
    {
        // TODO: Remove from inventory
    }

    public void SetEquippedArmour()
    {
        Item equippedItem = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().GetEquippedArmour();
        foreach(var btn in _SpawnedButtons)
        {
            var itemInstance = btn as InventoryItem;
            if(itemInstance.GetItem() == equippedItem)
                btn.Text += "*";
        }
    }
    



    public void RemoveItemButton(Button btn)
    {
        if (_SpawnedButtons.Contains(btn))
        {
            _SpawnedButtons.Remove(btn);
            btn.QueueFree();
        }
    }

    public void CloseInventory()
    {

        if (_SpawnedButtons.Count > 0)
        {
            foreach (var btn in _SpawnedButtons)
            {
                if(btn != null)
                   btn.QueueFree();
            }
        }

        _SpawnedButtons.Clear();

        GetNode<ColorRect>("ViewingItem").Hide();
        this.Hide();                // Hide the inventory

        

    }
}
