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
    

    // === MOVEMENT DETAILS === //
    private Vector2 _LastPosition;                      // Records the last position at the end of the frame


    // === AI === //
    Blackboard _Blackboard = new Blackboard();              // Reference to the blackboard



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
        // Get reference to the navigation system
        _Nav = GetNode<Navigation2D>("/root/Main/Navigation2D");

        SetupBlackboard();
        
        
    }
    
    

    public override void _Process(float delta)
    {
        
        base._Process(delta);

        if (_Player != null)
            _PlayerPosition = _Player.Position;

        if(_Blackboard.GetValueAsEnemyState("EnemyState") == EnemyState.ATTACKING)
        {
            _Blackboard.SetValueAsVector2("TargetLocation",
                GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController().Position);
        }

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        DetectPlayerAttack();
        FollowPlayer(delta);
        
    }

    /// <summary>
    /// Gets the player position and follows the player
    /// </summary>
    /// <param name="delta"></param>
    private void FollowPlayer(float delta)
    {
        if (_OwningCharacter.IsAlive() == false || !CanMove)
            return;

        var velocity = new Vector2();                   // New reference to the velocity

        // Create the path to the player
        _Paths = _Nav.GetSimplePath(GlobalPosition, _Blackboard.GetValueAsVector2("TargetLocation"), false);
        if (_Paths.Length > 1)
        {
            var distance = _Paths[1] - GlobalPosition;          // Get the distance
            var direction = distance.Normalized();              // Get the direction

            // Check the if the distance not close to the player 
            if (distance.Length() > 1.5 || _Paths.Length > 2)
            {
                // Get the direction to move in and apply the movement speed
                velocity = new Vector2(direction * (_MovementSpeed * delta));
            }
            else
            {
                // If within distance go to idle
                velocity = new Vector2();
            }

            Update();           // Refresh the update
        }

        MoveAndCollide(velocity);                 // move the character
        SetFacingDirection(velocity); 
        AnimationUpdate(velocity);
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
        if (distanceToPlayer < 20.0f && _CanAttack)
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

        _IsAttacking = true;                // Allow the character to attack again

        // Get reference to the player controller
        var player = GetNode<PlayerController>("/root/Main/Player");

        // Ensure the player controller is valid
        if (player != null)
        {
            PlayAttackAnimation();
            player.GetOwningCharacter()?.TakeDamage(GetOwningCharacter().GetCurrentAP());
            // TODO: Add Dodge feature for character
        }

        _CanAttack = false;
        
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
        if(_IsAttacking)
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
                // Spawn Level 1 Demon
                _OwningCharacter.SetupAI_Stats((float)GD.RandRange(25, 40), (float)GD.RandRange(25, 40));
                _OwningCharacter.SetEnemyType(EnemyType.WOLF_DEMON);
                LoadEnemySprite(EnemyType.WOLF_DEMON);
                EquipWeapon("Old Sword");
                
            } else if(roomLevel == 2)
            {
                float randValue = GD.Randf();
                if(randValue > 0.65f)
                {
                    // Setup the character
                    _OwningCharacter.SetEnemyType(EnemyType.WINGED_DEMON);
                    _OwningCharacter.SetupAI_Stats((float)GD.RandRange(30, 65), (float)GD.RandRange(30, 65));
                    LoadEnemySprite(EnemyType.WINGED_DEMON);
                    EquipWeapon("Old Sword");
                } else 
                {
                    // Setup the character
                    _OwningCharacter.SetupAI_Stats((float)GD.RandRange(25, 40), (float)GD.RandRange(25, 40));
                    _OwningCharacter.SetEnemyType(EnemyType.WOLF_DEMON);
                    LoadEnemySprite(EnemyType.WOLF_DEMON);

                    // Equip the weapon
                    EquipWeapon("The HellBard");
                }
            }
        }
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
}
