using Godot;
using System;
using System.Collections.Generic;


public class InventoryUI : ColorRect
{
    [Export] private PackedScene _ItemButton;           // Reference to the button that will spawn with it details

    private bool _CanOpen = true;
    private bool _CanClose = false;

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
                itemButton.Setup(item);
                SetEquippedWeapon();
            }
            
            SetEquippedWeapon();
               
            
            _SpawnedButtons.Add(btnNode);
        }
        
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

        this.Hide();                // Hide the inventory

        

    }
}
