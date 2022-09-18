using Godot;
using System;

public enum EnemyType
{
    WOLF_DEMON,
    WINGED_DEMON,
    FIRE_DEMON
}

[Serializable]
public class Character
{
    // === General Details === //
    protected bool _IsPlayer = false;                 // If this is controlled by the player or not
    protected string _CharacterName;
    protected EnemyType _EnemyType;
    
    [NonSerialized]
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
    protected float _CurrentXP = 0;
    protected float _NextLevelXP = 100;
    protected int _CurrentLevel = 1;

    public Character(string characterName = "Character", bool isPlayer = false, int level = 1)
    {
        _IsPlayer = isPlayer;
        _CharacterName = characterName;
        _CurrentHealth = _MaxHealth;
        UpdateHealthUI();
        _CurrentLevel = level;

        _Inventory = new InventoryComponent(this);

        SetupStats(level);           // Setup the stats of the characters 
        
        _CurrentAttackPoints = _MaxAttackPoints;
        _CurrentDefencePoints = _MaxDefencePoints;
        
        
    }

    public void SetupStats(int level = 1)
    {
        if (_IsPlayer)
        {
            _MaxAttackPoints = 75;
            _MaxDefencePoints = 75;
        }
    }

    public void SetupAI_Stats(float maxAttack, float maxDefence)
    {
        _MaxAttackPoints = maxAttack;
        _MaxDefencePoints = maxDefence;

        _CurrentAttackPoints = _MaxAttackPoints;
        _CurrentDefencePoints = _MaxDefencePoints;
    }  
    

    /// <summary>
    /// Applies damage to the characters health
    /// </summary>
    /// <param name="dp">Damage points</param>
    public void TakeDamage(float dp)
    {
        float ArmourMod = 1.0f;

        if(_Inventory.GetEquippedArmour() != null)
            ArmourMod = _Inventory.GetEquippedArmour().GenerateMod();       // Add armour modifier
        
        // Determine the points to be applied
        var percentage = _CurrentDefencePoints / 10;
        dp /= percentage;                   // Apply the amrour mod to the damage

        dp /= ArmourMod;
        _CurrentHealth -= dp;           // Reduce the health
        
        // Check if the player is still alive
        if (_CurrentHealth < 0)
        {
            _IsAlive = false;               // Player has died
            GetController().CanMove = false;
            _OwningController.TriggerDeathAnim();           // Trigger death animation
        } else 
        {
            _OwningController.TakeHit();
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

    public void AddXP(float xp)
    {
        if (xp <= 0)
            return;
        
        if (_CurrentXP + xp > _NextLevelXP)
        {
            var remainingXP = _NextLevelXP - (_CurrentXP + xp);
            LevelUp();
            AddXP(remainingXP);
        }
        else
        {
            _CurrentXP += xp;
        }

        var pc = _OwningController as PlayerController;
        pc?.GetUI()?.UpdateXP(_CurrentXP, _NextLevelXP, _CurrentLevel);
    }

    private void LevelUp()
    {
        _CurrentLevel++;
        _NextLevelXP = _NextLevelXP * 1.4f;
        _CurrentXP = 0;
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

    public void Respawn()
    {
        if (_OwningController is PlayerController)
        {
            _IsAlive = true;
            _CurrentHealth = _MaxHealth;
        }
    }

    public void SetOwningController(CharacterController controller) => _OwningController = controller;


    public float GetCurrentHealth() => _CurrentHealth;
    public float GetCurrentAP() => _CurrentAttackPoints;
    public float GetCurrentDP() => _CurrentDefencePoints;
    public bool IsAlive() => _IsAlive;
    public int GetCurrentLevel() => _CurrentLevel;
    public float GetMaxXP() => _NextLevelXP;
    public float GetCurrentXP() => _CurrentXP;
    public InventoryComponent GetInventory() => _Inventory;
    public CharacterController GetController() => _OwningController;
    public EnemyType GetEnemyType() => _EnemyType;
    public void SetEnemyType(EnemyType type) => _EnemyType = type; 
}