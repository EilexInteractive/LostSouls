using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public enum EnemyState
{
    NULL,
    ATTACKING,
    WANDERING,
    DEFENDING

}

public class EnemyController : CharacterController
{
    private Vector2 _PlayerPosition;                    // Reference to the player position
    private PlayerController _Player;                   // Reference to the player controller
    private Navigation2D _Nav;                          // Reference to the level navigation
    private Vector2[] _Paths = new Vector2[] { };           // Stores the path for pathfinding
    private NavAgent _NavAgent;                             // Reference to the navigation agent


    // === ATTACK SETTINGS === //
    private float _AttackDistance = 20.0f;
    private bool _HasAttackCooldown = false;
    private Timer _AttackCooldownTimer;
    

    // === MOVEMENT DETAILS === //
    private Vector2 _LastPosition;                      // Records the last position at the end of the frame


    // === AI === //
    Blackboard _Blackboard = new Blackboard();              // Reference to the blackboard
    private EilexFramework.AI.Tree _EnemyBT;



    public override void _Ready()
    {
        base._Ready();

        // Set Character & Controller
        int roomLevel = GetNode<SceneController>("/root/Main").GetRoomLevel();        
        _OwningCharacter = new Character("Demon", false, roomLevel);
        _OwningCharacter.SetOwningController(this);
        GenerateCharacter();
        // Get reference to the player controller
        _Player = GetNode<PlayerController>("/root/Main/Player");

        
        // Setup the navigation agent
        _NavAgent = GetNode<NavAgent>("NavAgent");
        _NavAgent.SetOwner(this);
        _NavAgent.SetMovementSpeed(_MovementSpeed);

        // Setup AI
        SetupBlackboard();
        _Blackboard.SetValueAsEnemyState("EnemyState", EnemyState.ATTACKING);
        _Blackboard.SetValueAsNode2D("Player", GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController());
        
    }
    
    

    public override void _Process(float delta)
    {
        
        base._Process(delta);

        // Update the player position within the blackboard so that we can access it from the BT
        if (_Player != null)
        {
            _PlayerPosition = _Player.Position;
            _Blackboard.SetValueAsVector2("TargetLocation", _PlayerPosition);
        }
            

        // Update the BT
        if(_EnemyBT != null)
            _EnemyBT.Update(delta);

        if(_HasAttackCooldown && _AttackCooldownTimer != null)
        {
            _AttackCooldownTimer.UpdateTimer(delta);
        }

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        DetectPlayerAttack();
        FollowPlayer(delta);
        
    }


    private void FollowPlayer(float delta)
    {
        SetFacingDirection(_NavAgent.GetMovementDirection());
        AnimationUpdate(_NavAgent.GetMovementDirection());
    }

    /// <summary>
    /// Sets the direction the character will be facing
    /// </summary>
    /// <param name="vel">Velocity to determine which way the enemy needs to face</param>
    private void SetFacingDirection(Vector2 vel)
    {
        // Return if the enemy is not moving
        if(vel == new Vector2())
            return;

        // Determine up/down direction first as non-priorty
        if(vel.y > 0)
        {
            _FacingDirection = FacingDirection.DOWN;
        } else 
        {
            _FacingDirection = FacingDirection.UP;
        }

        // Determine left/right direction as priorty
        if(vel.x > 0)
        {
            _FacingDirection = FacingDirection.RIGHT;
        } else if(vel.x < 0)
        {
            _FacingDirection = FacingDirection.LEFT;
        }
    }

    /// <summary>
    /// Detect if they are in range to attack the player
    /// </summary>
    private void DetectPlayerAttack()
    {
        // If the character is dead he can no longer attack
        if (!_OwningCharacter.IsAlive())
            return;
        
        // Update the player position
        _PlayerPosition = GetNode<PlayerController>("/root/Main/Player").Position;
        
        // Determine the distance 
        var distanceToPlayer = this.Position.DistanceTo(_PlayerPosition);

        // If close to the player start attacking
        if (distanceToPlayer < _AttackDistance && _CanAttack)
            Attack();
    }


    /// <summary>
    /// Determines the animation play based on the move direction of the enemy
    /// </summary>
    /// <param name="vel">Movement Direction</param>
    protected override void AnimationUpdate(Vector2 vel)
    {
        base.AnimationUpdate(vel);

        // Don't perform animations if the enemy is attacking or dead
        if (_OwningCharacter.IsAlive() == false || _IsAttacking)
            return;

        
        if(vel == new Vector2() || _LastPosition == this.Position)
        {
            switch(_FacingDirection)
            {
                case FacingDirection.LEFT:
                    _Anim.Play("IdleLeft");
                    break;
                case FacingDirection.RIGHT:
                    _Anim.Play("IdleRight");
                    break;
                case FacingDirection.DOWN:
                    _Anim.Play("IdleDown");
                    break;
                case FacingDirection.UP:
                    _Anim.Play("IdleUp");
                    break;
            }
        } else 
        {
            switch(_FacingDirection)
            {
                case FacingDirection.LEFT:
                    _Anim.Play("WalkLeft");
                    break;
                case FacingDirection.RIGHT:
                    _Anim.Play("WalkRight");
                    break;
                case FacingDirection.UP:
                    _Anim.Play("WalkUp");
                    break;
                case FacingDirection.DOWN:
                    _Anim.Play("WalkDown");
                    break;
            }
        }

        _LastPosition = this.Position;
        
    }

    public override void TriggerDeathAnim()
    {
        base.TriggerDeathAnim();
        GetNode<TextureProgress>("HealthBar").Hide();                               // Hide the texture progress
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;              // Disable the collision once they have died
        _Anim.ZIndex = -1;

    }

    public void AlreadyDead()
    {
        if(_OwningCharacter.IsAlive() == false)
        {
            _Anim.Play("AlreadyDead");
            GetNode<TextureProgress>("HealthBar").Hide();                               // Hide the texture progress
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;              // Disable the collision once they have died
            _Anim.ZIndex = -1;                  // Set the z index
        }
            
    }

    protected override void Attack()
    {
        
        base.Attack();

        if(!_CanAttack)
            return;

        _IsAttacking = true;                // Allow the character to attack again

        // Get reference to the player controller
        var player = GetNode<PlayerController>("/root/Main/Player");

        // Ensure the player controller is valid
        if (player != null)
        {
            PlayAttackAnimation();
            if(_OwningCharacter.GetEnemyType() != EnemyType.FIRE_DEMON)
                player.GetOwningCharacter()?.TakeDamage(GetOwningCharacter().GetCurrentAP());

            // TODO: Add Dodge feature for character
        }

        if(GetOwningCharacter().GetEnemyType() == EnemyType.FIRE_DEMON)
        {
            var fireBall = GD.Load<PackedScene>("res://Prefabs/FireBall.tscn");
            var fireBallInstance = fireBall.Instance();
            GetTree().Root.AddChild(fireBallInstance);
            var fireBallNode = fireBallInstance as Node2D;
            fireBallNode.Position = this.Position;
            fireBallNode.LookAt(GetNode<PlayerController>("/root/Main/Player").Position); 
            
        }

        _CanAttack = false;

        if(_HasAttackCooldown)
        {
            _AttackCooldownTimer = new Timer(5.0f, false, CompleteCooldownTimer);
        }

        
        
    }

    private void CompleteCooldownTimer()
    {
        _CanAttack = true;
        _AttackCooldownTimer = null;
    }

    private void PlayAttackAnimation()
    {
        switch(_FacingDirection)
        {
            case FacingDirection.LEFT:
                _Anim.Play("AttackLeft");
                break;
            case FacingDirection.RIGHT:
                _Anim.Play("AttackRight");
                break;
            case FacingDirection.DOWN:
                _Anim.Play("AttackDown");
                break;
            case FacingDirection.UP:
                _Anim.Play("AttackUp");
                break;
        }
    }
    
    /// <summary>
    /// When attack animation has finished reset the attack property
    /// </summary>
    public void OnAnimFinished()
    {
        if(_IsAttacking && !_HasAttackCooldown)
        {
            _IsAttacking = false;
        }
    }

    protected virtual void SetupBlackboard()
    {
        _Blackboard.SetValueAsNode2D("EnemyTarget", GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController());
        _Blackboard.SetValueAsEnemyState("EnemyState", EnemyState.ATTACKING);
    }

    private void GenerateCharacter()
    {
        var roomLevel = GetNode<SceneController>("/root/Main").GetRoomLevel();              // Get the level of the room

        // Ensure the owning character is valid
        if(_OwningCharacter != null)
        {
            // Check the room level to determine which type of character we wish to spawn
            if(roomLevel == 1)
            {
                SpawnWolfDemon();
                
            } else if(roomLevel == 2)
            {
                float randValue = GD.Randf();
                if(randValue > 0.65f)
                {
                    SpawnWingedDemon();
                } else 
                {
                    SpawnWolfDemon();
                }
            } else if(roomLevel == 3)
            {
                float randValue = GD.Randf();
                if(randValue > 0.80)
                {
                    SpawnWolfDemon();
                } else if(randValue > 0.60)
                {
                    SpawnFireDemon();
                } else 
                {
                    SpawnFireDemon();
                }
            }
        }
    }

    private void SpawnWolfDemon()
    {
        // Setup the character
        _OwningCharacter.SetupAI_Stats((float)GD.RandRange(25, 40), (float)GD.RandRange(25, 40));
        _OwningCharacter.SetEnemyType(EnemyType.WOLF_DEMON);
        LoadEnemySprite(EnemyType.WOLF_DEMON);

        _EnemyBT = new WolfDemonBT(this, _Blackboard);

        // Equip the weapon
        EquipWeapon("The HellBard");
    }

    private void SpawnWingedDemon()
    {
        // Setup the character
        _OwningCharacter.SetEnemyType(EnemyType.WINGED_DEMON);
        _OwningCharacter.SetupAI_Stats((float)GD.RandRange(30, 65), (float)GD.RandRange(30, 65));
        LoadEnemySprite(EnemyType.WINGED_DEMON);
        EquipWeapon("Old Sword");
    }

    private void SpawnFireDemon()
    {
        _OwningCharacter.SetEnemyType(EnemyType.FIRE_DEMON);
        LoadEnemySprite(EnemyType.FIRE_DEMON);
        _OwningCharacter.SetupAI_Stats((float)GD.RandRange(100, 150), (float)GD.RandRange(50, 70));
        _AttackDistance = 150.0f;
        _HasAttackCooldown = true;
        _AttackCooldown = 5.0f;
        
        EquipWeapon("Staff of Fire");
    }

    private void LoadEnemySprite(EnemyType type)
    {
        // Set the initial path
        string animationPath = "res://Animations/EnemyCharacter/";

        // Check what type of armour the demon is wearing
        if(_OwningCharacter.GetInventory().GetEquippedArmour() == null)
        {
            animationPath += "NoArmour/";
        }

        // Determine which type of enemy we need to load
        switch(type)
        {
            case EnemyType.WOLF_DEMON:
                animationPath += "Wolf_Demon.tres";
                break;
            case EnemyType.WINGED_DEMON:
                animationPath += "Winged_Demon.tres";
                break;
            case EnemyType.FIRE_DEMON:
                animationPath += "Fire_Demon.tres";
                break;

        }

        // Get the frames from the generated path
        _Anim.Frames = GD.Load<SpriteFrames>(animationPath);
    }

    private void EquipWeapon(string weaponName)
    {
        Weapon enemyWeapon = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem(weaponName) as Weapon;
        _OwningCharacter.GetInventory().AddItem(enemyWeapon);
        _OwningCharacter.GetInventory().EquipWeapon(enemyWeapon);
    }


    public AI_HealthBar GetHealthBar() => GetNode<AI_HealthBar>("HealthBar") as AI_HealthBar;
    public int GetCharacterLevel() => _OwningCharacter.GetCurrentLevel();
    public Blackboard GetBlackboard() => _Blackboard;
    public NavAgent GetNavAgent() => _NavAgent;
}
