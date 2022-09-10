using Godot;
using System;

public enum FacingDirection
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class CharacterController : KinematicBody2D
{
    protected Character _OwningCharacter;             // Reference to the character that owns this controller
    protected AnimatedSprite _Anim;
    public bool CanMove;
    protected FacingDirection _FacingDirection;           // Direction we are facing

    protected bool _IsAttacking = false;


    [Export] protected float _AttackCooldown;   
    protected Timer AttackCooldownTimer;
    protected bool _CanAttack = true;
    
    [Export] protected float _MovementSpeed = 1;              // How fast the character will move
    [Export] protected float _AttackAnimSpeed;

    protected AudioStreamPlayer2D _SwordAudio;

    protected bool EnableForce;
    public Vector2 ApplyingForce = new Vector2();
    
    public override void _Ready()
    {
        base._Ready();
        
        // Get the reference to the animated sprite
        _Anim = GetNode<AnimatedSprite>("AnimatedSprite");
        _SwordAudio = GetNode<AudioStreamPlayer2D>("SwordAudio");
    }

    public virtual void TriggerDeathAnim()
    {
        if (_Anim != null)
        {
            _Anim.Play("Death");
        }
    }

    protected virtual void Attack()
    {
        if (!_CanAttack)
            return;
        
        AttackCooldownTimer = new Timer(_AttackCooldown, false, EnableAttack);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if(AttackCooldownTimer != null)
            AttackCooldownTimer.UpdateTimer(delta);
        
        if (ApplyingForce != new Vector2())
        {
            Position += ApplyingForce;
            ApplyingForce = new Vector2();
        }
    }

    protected virtual void AnimationUpdate(Vector2 vel)
    {
        if (!_OwningCharacter.IsAlive())
            return;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (_OwningCharacter != null && !_OwningCharacter.IsAlive())
            return;
    }

    public void EnableAttack() => _CanAttack = true;


    // === GETTERS & SETTERS === //
    public void SetOwningCharacter(Character character, bool NewGame = true)
    {
        _OwningCharacter = character;
        if (!NewGame)
        {
            if (_OwningCharacter.IsAlive() == false)
            {
                CanMove = false;
                if (this is EnemyController)
                {
                    var ec = this as EnemyController;
                    ec?.AlreadyDead();
                    ec?.GetHealthBar().Hide();
                    GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
                }
            }
            else
            {

            }
            
        }
    }

    public virtual void SetEquippedItem(Weapon weapon)
    {
        
    }

    public void SetEquippedArmour(Armour armour)
    {
        _Anim.SetSpriteFrames(GD.Load<SpriteFrames>("res://Animations/Armour/" + armour.GetItemName() + ".tres"));
    }
    
    public Character GetOwningCharacter() => _OwningCharacter;
    public float GetAttackCooldown() => _AttackCooldown;

}