using Godot;
using System;

public class ItemPickup : Area2D, IInteractable
{
    public Item OwningItem;
    [Export] private string LoadItemName = "";
    [Export] private ItemRarity _itemRarity = ItemRarity.Null;
    private Sprite _ItemSprite;

    public override void _Ready()
    {
        base._Ready();
        
        _ItemSprite = GetNode<Sprite>("Sprite");                // Get reference to the owning sprite
        if (LoadItemName != "")
        {
            _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/" + LoadItemName + ".png");
            if (_itemRarity == ItemRarity.Null)
            {
                OwningItem = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem(LoadItemName);
            }
            else
            {
                OwningItem = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem(LoadItemName, _itemRarity);
            }
        }
    }

    public void SetItem(Item item)
    {
        OwningItem = item;
        if(item.GetItemType() != ItemType.Armour)
        {
            _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/" + item.GetItemName() + ".png");
        } else 
        {
            switch(item.GetItemRarity())
            {
                case ItemRarity.Common:
                    _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/CommonArmour.png");
                    break;
                case ItemRarity.Uncommon:
                    _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/UncommmonArmour.png");
                    break;
                case ItemRarity.Rare:
                    _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/RareArmour.png");
                    break;
                case ItemRarity.Legendary:
                    _ItemSprite.Texture = GD.Load<Texture>("res://Sprites/Items/LengendArmour");
                    break;


            }
        }
    }

    

    public void OnBodyEnter(Node node)
    {
        // Check it's the player entering
        if (node is PlayerController)
        {
            var player = node as PlayerController;          // Get the controller
            player?.GetUI()?.ToggleItemInfo(true, OwningItem);
            player._InteractableItem = this;                // Set this is the interactable item
        }
    }

    public void OnBodyExit(Node node)
    {
        // Check it's the player controller that we are getting
        if (node is PlayerController)
        {
            var player = node as PlayerController;                  // Get the player controller
            player?.GetUI()?.ToggleItemInfo(false);              // Hide the prompt to pickup
            player._InteractableItem = null;                        // Empty the interactable item
        }
    }
}
