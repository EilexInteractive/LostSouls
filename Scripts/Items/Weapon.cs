using Godot;
using System;

[Serializable]
public class Weapon : Item
{
    protected float _AttackModifierMin;                
    protected float _AttackModifierMax;
    protected float _CooldownTimer;
    
    public Weapon(string name, string desc, int cost, Character owner = null, float minAttack = 1.0f, float maxAttack = 1.0f, float cooldown = 0.3f) : base(name, desc, cost, ItemType.Weapon)
    {
        _AttackModifierMin = minAttack;
        _AttackModifierMax = maxAttack;
        _Owner = owner;
    }

    public override void UseItem()
    {
        _Owner.GetInventory().EquipWeapon(this);
    }

    public float GenerateDP() => (float)GD.RandRange(_AttackModifierMin, _AttackModifierMax);
    public float GetCooldown() => _CooldownTimer;
}