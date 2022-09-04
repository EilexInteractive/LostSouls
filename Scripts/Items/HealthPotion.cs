using Godot;
using System;

[Serializable]
public class HealthPotion : Item
{
    private float _HP_Amount;
    public HealthPotion(float hp) : base("HP Potion", $"Increase HP by {hp}", 15, ItemType.Consumable)
    {
        _HP_Amount = hp;
    }

    public override void UseItem()
    {
        if (_Owner.IncreaseHealth(_HP_Amount))
        {
            _Owner.GetInventory().RemoveItem(this);
        }
    }
}