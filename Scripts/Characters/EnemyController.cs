using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class EnemyController : CharacterController
{
    private Vector2 _PlayerPosition;                    // Reference to the player position
    private PlayerController _Player;
    private Navigation2D _Nav;
    private Vector2[] _Paths = new Vector2[] { };
    

    // === MOVEMENT DETAILS === //
    private Vector2 _LastPosition;



    public override void _Ready()
    {
        base._Ready();

        // Set Character & Controller
        int roomLevel = GetNode<SceneController>("/root/Main").GetRoomLevel();
        _OwningCharacter = new Character("Pirate", false, roomLevel);
        _OwningCharacter.SetOwningController(this);
        _Player = GetNode<PlayerController>("/root/Main/Player");
        _Nav = GetNode<Navigation2D>("/root/Main/Navigation2D");
        

        // Create a sword & Equip it
        Weapon weapon = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem("Old Sword") as Weapon;
        _OwningCharacter.GetInventory().AddItem(weapon);
        _OwningCharacter.GetInventory().EquipWeapon(weapon); 
        
        
    }
    
    

    public override void _Process(float delta)
    {
        
        base._Process(delta);

        if (_Player != null)
            _PlayerPosition = _Player.Position;

    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        
        DetectPlayerAttack();
        FollowPlayer(delta);
        
    }

    private void FollowPlayer(float delta)
    {
        if (_OwningCharacter.IsAlive() == false || !CanMove)
            return;

        var velocity = new Vector2();

        _Paths = _Nav.GetSimplePath(GlobalPosition, _PlayerPosition, false);
        if (_Paths.Length > 1)
        {
            var distance = _Paths[1] - GlobalPosition;
            var direction = distance.Normalized();
            if (distance.Length() > 1.5 || _Paths.Length > 2)
            {
                velocity = new Vector2(direction * (_MovementSpeed * delta));
            }
            else
            {
                velocity = new Vector2();
            }

            Update();
        }

        MoveAndSlide(velocity);
        SetFacingDirection(velocity);
        AnimationUpdate(velocity);
    }

    private void SetFacingDirection(Vector2 vel)
    {
        if(vel == new Vector2())
            return;

        if(vel.y > 0)
        {
            _FacingDirection = FacingDirection.DOWN;
        } else 
        {
            _FacingDirection = FacingDirection.UP;
        }

        if(vel.x > 0)
        {
            _FacingDirection = FacingDirection.RIGHT;
        } else if(vel.x < 0)
        {
            _FacingDirection = FacingDirection.LEFT;
        }
    }

    private void DetectPlayerAttack()
    {
        // If the character is dead he can no longer attack
        if (!_OwningCharacter.IsAlive())
            return;
        _PlayerPosition = GetNode<PlayerController>("/root/Main/Player").Position;
        var distanceToPlayer = this.Position.DistanceTo(_PlayerPosition);

        if (distanceToPlayer < 20.0f && _CanAttack)
            Attack();
    }

    protected override void AnimationUpdate(Vector2 vel)
    {
        base.AnimationUpdate(vel);

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

    }

    public void AlreadyDead()
    {
        if(_OwningCharacter.IsAlive() == false)
        {
            _Anim.Play("AlreadyDead");
            GetNode<TextureProgress>("HealthBar").Hide();                               // Hide the texture progress
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;              // Disable the collision once they have died
        }
            
    }

    protected override void Attack()
    {
        
        base.Attack();

        _IsAttacking = true;

        var player = GetNode<PlayerController>("/root/Main/Player");
        if (player != null)
        {
            PlayAttackAnimation();
            player.GetOwningCharacter()?.TakeDamage(GetOwningCharacter().GetCurrentAP());
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
    
    public void OnAnimFinished()
    {
        if(_IsAttacking)
        {
            _IsAttacking = false;
        }
    }


    public AI_HealthBar GetHealthBar() => GetNode<AI_HealthBar>("HealthBar") as AI_HealthBar;
    public int GetCharacterLevel() => _OwningCharacter.GetCurrentLevel();
}
