using Godot;
using System;

public class AI_HealthBar : TextureProgress
{
    // Updates the AI's health bar
    public void UpdateHealth(float hp) => this.Value = hp;
}
