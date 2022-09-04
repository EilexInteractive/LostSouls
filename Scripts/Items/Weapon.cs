using Godot;
using System;

[Serializable]
public class Weapon : Item
{
    private Character _OwningCharacter;
    protected float _AttackModifierMin;                
    protected float _AttackModifierMax;                 
    
    public Weapon(string name, string desc, int cost, Character owner = null, float minAttack = 1.0f, float maxAttack = 1.0f) : base(name, desc, cost, ItemType.Weapon)
    {
        _AttackModifierMin = minAttack;
        _AttackModifierMax = maxAttack;
        _OwningCharacter = owner;
    }

    public override void UseItem()
    {
        _OwningCharacter.GetInventory().EquipWeapon(this);
    }

    public float GenerateDP() => (float)GD.RandRange(_AttackModifierMin, _AttackModifierMax);
}