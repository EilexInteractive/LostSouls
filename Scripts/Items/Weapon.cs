using Godot;
using System;

public enum WeaponType
{
    LONG_SWORD,
    AXE,
    WARHAMMER,
    WAR_AXE,
    HALBERD
}

[Serializable]
public class Weapon : Item
{
    protected float _AttackModifierMin;                
    protected float _AttackModifierMax;
    protected float _CooldownTimer;

    protected WeaponType _WeaponType;
    
    public Weapon(string name, string desc, int cost, WeaponType type, Character owner = null, float minAttack = 1.0f, float maxAttack = 1.0f, float cooldown = 0.3f, ItemRarity rare = ItemRarity.Common) : base(name, desc, cost, ItemType.Weapon, rare)
    {
        _AttackModifierMin = minAttack;
        _AttackModifierMax = maxAttack;
        _Owner = owner;
        _WeaponType = type;
    }

    public override void UseItem()
    {
        _Owner.GetInventory().EquipWeapon(this);
    }

    public float GenerateDP() => (float)GD.RandRange(_AttackModifierMin, _AttackModifierMax);
    public float GetCooldown() => _CooldownTimer;
    public WeaponType GetWeaponType() => _WeaponType;
}