using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EilexFramework.AI;

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
    private Blackboard _Blackboard = new Blackboard();              // Reference to the blackboard
    private FiniteStateMachine FSM;
    



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
        if(FSM != null)
            FSM.Update(delta);

        if(_HasAttackCooldown && _AttackCooldownTimer != null)
        {
            _AttackCooldownTimer.UpdateTimer(delta);
        }

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        UpdateDirection();
        
    }


    private void UpdateDirection()
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
    /// Determines the animation play based on the move direction of the enemy
    /// </summary>
    /// <param name="vel">Movement Direction</param>
    protected override void AnimationUpdate(Vector2 vel)
    {
        base.AnimationUpdate(vel);

        // Don't perform animations if the enemy is attacking or dead
        if (_OwningCharacter.IsAlive() == false || _Blackboard.GetValueAsBool("IsAttacking") == true)
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

    public void StartAttackCooldown()
    {
        _AttackCooldownTimer = new Timer(_AttackCooldown, false, CompleteCooldownTimer);
    }

    private void CompleteCooldownTimer()
    {
        _Blackboard.SetValueAsBool("CanAttack", true);
        _AttackCooldownTimer = null;
        GetBlackboard().SetValueAsBool("IsAttacking", false);
    }

    public void PlayAttackAnimation()
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
        if(_Blackboard.GetValueAsBool("IsAttacking") && !_HasAttackCooldown)
        {
             _Blackboard.SetValueAsBool("IsAttacking", false);
         }
    }

    protected virtual void SetupBlackboard()
    {
        _Blackboard.SetValueAsNode2D("EnemyTarget", GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController());
        _Blackboard.SetValueAsEnemyState("EnemyState", EnemyState.ATTACKING);
        _Blackboard.SetValueAsBool("IsAlive", true);
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
        _HasAttackCooldown = true;                     
        _AttackCooldown = 0.3f;   
        EquipWeapon("Old Sword");
                     
        

        // Setup Blackboard
        _Blackboard.SetValueAsFloat("AttackDistance", 25.0f);
        _Blackboard.SetValueAsBool("CanAttack", true);
        _Blackboard.SetValueAsBool("IsAttacking", false);

        // Setup state machinex
        FSM = new BaseEnemyFSM(this);
        FSM.AddState(new FollowPlayerState(this));
        FSM.AddState(new AttackState(this));
        FSM.SetState("FollowPlayer");
        
        
    }

    private void SpawnWingedDemon()
    {
        // Setup the character
        _OwningCharacter.SetEnemyType(EnemyType.WINGED_DEMON);
        _OwningCharacter.SetupAI_Stats((float)GD.RandRange(30, 65), (float)GD.RandRange(30, 65));
        LoadEnemySprite(EnemyType.WINGED_DEMON);
        // Equip the weapon
        EquipWeapon("The HellBard");
        

        _Blackboard.SetValueAsFloat("AttackDistance", 25.0f);
        _Blackboard.SetValueAsBool("CanAttack", true);
        _Blackboard.SetValueAsBool("IsAttacking", false);

        // Setup state machinex
        FSM = new BaseEnemyFSM(this);
        FSM.AddState(new FollowPlayerState(this));
        FSM.AddState(new AttackState(this));
        FSM.SetState("FollowPlayer");
    }

    private void SpawnFireDemon()
    {
        _OwningCharacter.SetEnemyType(EnemyType.FIRE_DEMON);
        LoadEnemySprite(EnemyType.FIRE_DEMON);
        _OwningCharacter.SetupAI_Stats((float)GD.RandRange(100, 150), (float)GD.RandRange(50, 70));
        _AttackDistance = 150.0f;
        _HasAttackCooldown = true;
        _AttackCooldown = 5.0f;

        // Setup Blackboard
        _Blackboard.SetValueAsFloat("AttackDistance", 150.0f);
        _Blackboard.SetValueAsBool("CanAttack", true);
        _Blackboard.SetValueAsBool("IsAttacking", false);

        // Setup state machinex
        FSM = new BaseEnemyFSM(this);
        FSM.AddState(new FireballAttack(this, GetTree().Root));
        
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
    public FiniteStateMachine GetFSM() => FSM;
}
