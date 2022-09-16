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
        // Enable the item view
        GetNode<ColorRect>("ViewingItem").Show();
        _ViewingItem = item;
        _ViewingBtn = itemBtn;
        
        // Setup the item details
        GetNode<TextureRect>("ViewingItem/ItemIcon").Texture = GD.Load<Texture>(GetPathToIcon(item));
        GetNode<Label>("ViewingItem/ItemName").Text = item.GetItemName();
        GetNode<RichTextLabel>("ViewingItem/ItemDescription").BbcodeText = "[center]" + item.GetItemDesc() + "[/center]";
        

        var gc = GetNode<GameController>("/root/GameController");
        var actionBtn = GetNode<Button>("ViewingItem/HBoxContainer/ActionBtn");

        if(gc != null)
        {
            if(item.GetItemType() == ItemType.Weapon)
            {
                if(item == gc.GetPlayerCharacter().GetInventory().GetEquippedWeapon())
                {
                    actionBtn.Hide();
                } else 
                {
                    actionBtn.Show();
                    actionBtn.Text = "Equip Item";
                }

                var weapon = item as Weapon;
                var maxAttack = weapon.GetMaxAttack() * 10;
                var attackString = maxAttack.ToString();
                var finalValue = attackString.Split('.');
                GetNode<Label>("ViewingItem/ItemProperty").Text = $"Attack Boost: {finalValue[0]} %";
            } else if(item.GetItemType() == ItemType.Armour)
            {
                if(item == gc.GetPlayerCharacter().GetInventory().GetEquippedArmour())
                {
                    actionBtn.Hide();
                } else 
                {
                    actionBtn.Show();
                    actionBtn.Text = "Equip Item";
                }
                var armour = item as Armour;
                var maxDefence = (armour.GetMaxDefence() * 10).ToString();
                var finalValue = maxDefence.Split('.');
                GetNode<Label>("ViewingItem/ItemProperty").Text = $"Defence Boost: {finalValue[0]} %";
            } else if(item.GetItemType() == ItemType.Consumable)
            {
                actionBtn.Show();
                actionBtn.Text = "Consume";
                var hp = item as HealthPotion;
                var hp_value = hp.GetHP().ToString();
                var finalValue = hp_value.Split('.');
                GetNode<Label>("ViewingItem/ItemProperty").Text = $"Health Boost: {finalValue[0]} %";
            }
        }
        
    }

    private string GetPathToIcon(Item item)
    {
        var iconPath = $"res://Sprites/Icons/{item.GetItemType()}s/";
        switch(item.GetItemRarity())
        {
            case ItemRarity.Common:
                iconPath += "Common/";
                break;
            case ItemRarity.Uncommon:
                iconPath += "Uncommon/";
                break;
            case ItemRarity.Rare:
                iconPath += "Rare/";
                break;
            case ItemRarity.Legendary:
                iconPath += "Legendary/";
                break;
        }

        
        return iconPath + item.GetItemName() + ".png";
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
            GetNode<InventoryUI>("/root/Main/Player/Canvas/CanvasLayer/InventoryRect").RemoveItemButton(_ViewingBtn);
            // Hide the viewing panel as the item no longer exist
            GetNode<ColorRect>("CanvasLayer/InventoryRect/ViewingItem").Hide();
        } else if (_ViewingItem.GetItemType() == ItemType.Weapon)
        {
            // If it's a weapon than equip the weapon & update the inventory
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().EquipWeapon(_ViewingItem as Weapon);
            GetNode<InventoryUI>("/root/Main/Player/Canvas/CanvasLayer/InventoryRect").SetEquippedWeapon();
            // Hide the button as we have now equipped the item
            GetNode<Button>("CanvasLayer/InventoryRect/HBoxContainer/ActionBtn").Hide();
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
