﻿using Godot;
using System;

[Serializable]
public class Character
{
    // === General Details === //
    protected bool _IsPlayer = false;                 // If this is controlled by the player or not
    protected string _CharacterName;
    protected CharacterController _OwningController;
    protected InventoryComponent _Inventory;
    
    // === Health Settings === //
    protected float _MaxHealth = 100;
    protected float _CurrentHealth;
    protected bool _IsAlive = true;
    
    // === Attack Stats === //
    protected float _MaxAttackPoints;
    protected float _CurrentAttackPoints;
    
    // === Defence Stats === //
    protected float _MaxDefencePoints;
    protected float _CurrentDefencePoints;
    
    // === XP Point System === //
    protected float _CurrrentXP;
    protected float _NextLevelXP;
    protected int _CurrentLevel;

    public Character(string characterName = "Character", bool isPlayer = false)
    {
        _IsPlayer = isPlayer;
        _CharacterName = characterName;
        _CurrentHealth = _MaxHealth;
        UpdateHealthUI();

        _Inventory = new InventoryComponent(this);

        SetupStats();           // Setup the stats of the characters 
        
        _CurrentAttackPoints = _MaxAttackPoints;
        _CurrentDefencePoints = _MaxDefencePoints;
        
        
    }

    public void SetupStats()
    {
        if (_IsPlayer)
        {
            _MaxAttackPoints = 35;
            _MaxDefencePoints = 35;
        }
        else
        {
            // TODO: Replace this with random stats
            _MaxAttackPoints = 20;
            _MaxDefencePoints = 30;
        }
    }
    

    /// <summary>
    /// Applies damage to the characters health
    /// </summary>
    /// <param name="dp">Damage points</param>
    public void TakeDamage(float dp)
    {
        //dp *= _Inventory.GetEquippedWeapon().GenerateDP();              // Add the sword modifier to the damage
        
        // Determine the points to be applied
        var percentage = _CurrentDefencePoints / 10;
        dp /= percentage;
        

        _CurrentHealth -= dp;           // Reduce the health
        
        // Check if the player is still alive
        if (_CurrentHealth < 0)
        {
            _IsAlive = false;               // Player has died
            GetController().CanMove = false;
            _OwningController.TriggerDeathAnim();           // Trigger death animation
        }
        
        UpdateHealthUI();
            
    }

    public bool IncreaseHealth(float amt)
    {
        // Check that we need to use the HP
        if (_CurrentHealth >= _MaxHealth)
            return false;

        _CurrentHealth += amt;                  // Increase the health points
        // Check that we haven't passed the max HP
        // if so set it to the max HP
        if (_CurrentHealth > _MaxHealth)
            _CurrentHealth = _MaxHealth;
        
        UpdateHealthUI();

        return true;
    }

    private void UpdateHealthUI()
    {
        if (_OwningController is PlayerController)
        {
            var controller = _OwningController as PlayerController;
            controller?.GetUI().UpdateHealthBar(_CurrentHealth);
        } else if (_OwningController is EnemyController)
        {
            var controller = _OwningController as EnemyController;
            controller?.GetHealthBar().UpdateHealth(_CurrentHealth);
        }

    }


    public void SetOwningController(CharacterController controller) => _OwningController = controller;


    public float GetCurrentHealth() => _CurrentHealth;
    public float GetCurrentAP() => _CurrentAttackPoints;
    public float GetCurrentDP() => _CurrentDefencePoints;
    public bool IsAlive() => _IsAlive;
    public int GetCurrentLevel() => _CurrentLevel;
    public InventoryComponent GetInventory() => _Inventory;
    public CharacterController GetController() => _OwningController;
}