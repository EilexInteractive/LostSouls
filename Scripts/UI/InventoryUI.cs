using Godot;
using System;
using System.Collections.Generic;


public class InventoryUI : ColorRect
{
    [Export] private PackedScene _ItemButton;           // Reference to the button that will spawn with it details
    private Timer _CloseLimitTimer;                 // Timer to prevent flickering of the UI
    private Timer _OpenLimitTimer;                  // Timer to prevent flickering of the UI

    private bool _CanOpen = true;
    private bool _CanClose = false;

    private List<Button> _SpawnedButtons = new List<Button>();

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        _CloseLimitTimer?.UpdateTimer(delta);
        _OpenLimitTimer?.UpdateTimer(delta);
    }


    public void OpenInventory(InventoryComponent inventory)
    {
        if (_CanOpen == false)
            return;
        
        
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
            if(itemButton != null)
                itemButton.Setup(item);
            
            _SpawnedButtons.Add(btnNode);
        }

        _CanClose = false;
        _CanOpen = false;
        _CloseLimitTimer = new Timer(0.3f, false, SetCanCloseTimer);
    }

    public void SetCanCloseTimer()
    {
        _CanClose = true;
        _CloseLimitTimer = null;
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
        if (!_CanClose)
            return;

        if (_SpawnedButtons.Count > 0)
        {
            foreach (var btn in _SpawnedButtons)
            {
                if(btn != null)
                   btn.QueueFree();
            }
        }

        _SpawnedButtons.Clear();
        
        _CanOpen = false;
        _CanClose = false;
        _OpenLimitTimer = new Timer(0.3f, false, SetCanOpenTimer);

        this.Hide();                // Hide the inventory

        

    }

    public void SetCanOpenTimer()
    {
        _CanOpen = true;
        _OpenLimitTimer = null;
    }
}
