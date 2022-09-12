using Godot;
using System;

[Serializable]
public class Armour : Item
{
    protected float _MinDamageMod;
    protected float _MaxDamageMod;

    public Armour(string name, string desc, int cost, ItemType type, float minMod = 1.0f, float maxMod = 1.3f, ItemRarity rare = ItemRarity.Common) : base(name, desc, cost, type, rare)
    {
        _MinDamageMod = minMod;
        _MaxDamageMod = maxMod;
    }

    public override void UseItem()
    {
        _Owner.GetController().SetEquippedArmour(this);
    }

    public float GenerateMod()
    {
        GD.Randomize();
        return (float)GD.RandRange(_MinDamageMod, _MaxDamageMod);
    }
}