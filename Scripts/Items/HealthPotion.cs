using Godot;
using System;

[Serializable]
public class HealthPotion : Item
{
    private float _HP_Amount;
    public HealthPotion(float hp, ItemRarity rare) : base("HP Potion", $"Increase HP by {hp}", 15, ItemType.Consumable, rare)
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

    public float GetHP() => _HP_Amount;
}