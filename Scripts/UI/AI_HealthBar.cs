using Godot;
using System;

public class AI_HealthBar : TextureProgress
{
    public void UpdateHealth(float hp) => this.Value = hp;
}
