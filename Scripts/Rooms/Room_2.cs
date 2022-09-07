using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Room_2 : SceneController
{
    public override void _Ready()
    {
        base._Ready();

        var player = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;
        player.GetUI().SetMessage("Man there's so many demons in here. This is gonna be a challenge...");
    }
}
