using Godot;
using System;



public class LootChest : Area2D, IInteractable
{
    public event Action OnChestOpen;
    
    
    public bool IsOpen;
    private AnimatedSprite _Anim;

    private InventoryComponent _Inventory;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _Inventory = new InventoryComponent(null);          // Create the inventory component
        _Anim = GetNode<AnimatedSprite>("AnimatedSprite");              // Get reference to the animator
        GenerateLoot();
        
        
    }

    public void OnBodyEntered(Node body)
    {
        if (body is PlayerController)
        {
            var pc = body as PlayerController;              // Get the colliding body as player character
            pc?.GetUI().TogglePickupLabel(true, "Open Chest");              // Prompt the player to open the chest
            pc._InteractableItem = this;                    // Set this is the interaction for the player
        }
    }

    public void OnBodyExit(Node node)
    {
        if (node is PlayerController)
        {
            var pc = node as PlayerController;                  // Get the exit body as a player controller
            pc?.GetUI().TogglePickupLabel(false, "");       // Hide any prompts
            pc._InteractableItem = null;                    // Remove this as an interaction for the character

        }
    }

    public void OpenChest()
    {
        if(!IsOpen)
            _Anim.Play("Open");


        OnChestOpen?.Invoke();
    }

    private void GenerateLoot()
    {
        // TODO: Load from a loot table
    }
    
}
