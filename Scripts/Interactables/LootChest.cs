using Godot;
using System;



public class LootChest : Area2D, IInteractable
{
    public event Action OnChestOpen;
    
    
    public bool IsOpen;
    private AnimatedSprite _Anim;

    private InventoryComponent _Inventory;
    [Export] private bool _ContainsLoot = true;                    // If the loot chest contains loot to spawn
    [Export] private int _MinItems = 0;                             // Min amount of items that will spawn
    [Export] private int _MaxItems = 3;                             // Max amount of items that will spawn
    [Export] private ItemRarity _ChestRarity;                       // What type of items are contained inside
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _Inventory = new InventoryComponent(null);          // Create the inventory component
        _Anim = GetNode<AnimatedSprite>("AnimatedSprite");              // Get reference to the animator
    }

    public void OnBodyEntered(Node body)
    {
        if (IsOpen)
            return;
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
            pc?.GetUI().TogglePickupLabel(false);       // Hide any prompts
            pc._InteractableItem = null;                    // Remove this as an interaction for the character

        }
    }

    public void OpenChest()
    {
        if (IsOpen)
            return;
        
        // Play the animation that opens the chest
        if(!IsOpen)
            _Anim.Play("Open");

        IsOpen = true;              // Set the chest to open
        
        // Perform the event on the chest and than set it to null for future use
        OnChestOpen?.Invoke();
        OnChestOpen = null;

        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        


        // If the chest doesn't contain any loot, than skip the rest of the code.
        if (!_ContainsLoot)
            return;

        GD.Randomize();
        float randValue = (float)GD.RandRange(1, 4);                // Amount of spawned item

        
        for (int i = 1; i < (int)randValue; ++i)
        {
            var db = GetNode<ItemDatabase>("/root/ItemDatabase");
            if (db != null)
            {
                GD.Print("Hello World");
                Item item = db.GetRandomItem(_ChestRarity);
                SpawnItem(item);
            }
        }
        
    }
    

    private void SpawnItem(Item item)
    {
        // Get the pickup item & create an instance of it
        PackedScene itemScene = GD.Load<PackedScene>("res://Prefabs/PickupItem.tscn");
        var itemInstance = itemScene.Instance();
        GetNode<Node2D>("/root/Main").AddChild(itemInstance);
        var itemDetails = itemInstance as ItemPickup;
        itemDetails.SetItem(item);

        Vector2 SpawnPoint = new Vector2
        {
            x = Position.x + ((float)GD.RandRange(-25, 25)),
            y = Position.y + ((float)GD.RandRange(-25, 25))
        };

        itemDetails.Position = SpawnPoint;
    }
}
