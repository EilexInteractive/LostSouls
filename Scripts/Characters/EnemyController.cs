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
        Weapon weapon = new Weapon("Basic Sword", "This is a basic sword", 3, _OwningCharacter);
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
        AnimationUpdate(velocity);
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

        if (_OwningCharacter.IsAlive() == false)
            return;

        if (vel == new Vector2())
        {
            _Anim.Play("Idle");
            
        }
        else
        {
            _Anim.Play("Walk");
        }
    }

    public override void TriggerDeathAnim()
    {
        base.TriggerDeathAnim();
        GetNode<TextureProgress>("HealthBar").Hide();                               // Hide the texture progress
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;              // Disable the collision once they have died

    }

    public void AlreadyDead()
    {
        _Anim.Play("AlreadyDead");
    }

    protected override void Attack()
    {
        
        base.Attack();

        var player = GetNode<PlayerController>("/root/Main/Player");
        if (player != null)
        {
            _SwordAnim.Play("Swing");
            player.GetOwningCharacter()?.TakeDamage(GetOwningCharacter().GetCurrentAP());
        }

        _CanAttack = false;
        
    }

    public AI_HealthBar GetHealthBar() => GetNode<AI_HealthBar>("HealthBar") as AI_HealthBar;
    public int GetCharacterLevel() => _OwningCharacter.GetCurrentLevel();
}
