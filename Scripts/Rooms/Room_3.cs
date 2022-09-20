using Godot;
using System;

public class Room_3 : SceneController
{
    public override void _Ready()
    {
        base._Ready();

        var playerController = GetNode<PlayerController>("Player");
        playerController.GetUI().FollowUpMessageEvent += FollowUpEventMessage;

        if(playerController.Position == new Vector2(0, 0))
            GD.Print("Zerooooooo");
    }

    public void FollowUpEventMessage()
    {
        var playerController = GetNode<PlayerController>("Player");
        playerController.GetUI().SetMessage("Here's 2 health potions, I'm sure I'll see you soon...");
        var playerCharacter = playerController.GetOwningCharacter();

        // Get 2 new health potions
        var healthPotion_1 = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem("HP Potion", ItemRarity.Uncommon);
        var healthPotion_2 = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem("HP Potion", ItemRarity.Uncommon);

        // Add the health potions to the inventory
        playerCharacter.GetInventory().AddItem(healthPotion_1);
        playerCharacter.GetInventory().AddItem(healthPotion_2);

    }
}
