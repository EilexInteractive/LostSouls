using Godot;
using System;

public class CharacterController : KinematicBody2D
{
    protected Character _OwningCharacter;             // Reference to the character that owns this controller
    protected AnimatedSprite _Anim;
    public bool CanMove;
    protected Sprite _SwordSprite;                    // Reference to the sword sprite
    
        
        
    protected AnimationPlayer _SwordAnim;
    

    [Export] protected float _AttackCooldown;   
    protected Timer AttackCooldownTimer;
    protected bool _CanAttack = true;
    
    [Export] protected float _MovementSpeed = 1;              // How fast the character will move
    [Export] protected float _AttackAnimSpeed;

    protected bool EnableForce;
    public Vector2 ApplyingForce = new Vector2();
    
    public override void _Ready()
    {
        base._Ready();
        
        
        // Get reference to the sword animation player
        _SwordAnim = GetNode<AnimationPlayer>("AnimationPlayer");
        // Reference to the sword sprite
        _SwordSprite = GetNode<Sprite>("AnimatedSprite/Sword");
        // Set the speed that the sword animation will play at
        _SwordAnim.PlaybackSpeed = _AttackAnimSpeed;
        
        // Get the reference to the animated sprite
        _Anim = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public virtual void TriggerDeathAnim()
    {
        if (_Anim != null)
        {
            _SwordSprite.Hide();
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
                CanMove = false;
            
            TriggerDeathAnim();
        }
    }
    public Character GetOwningCharacter() => _OwningCharacter;
    public float GetAttackCooldown() => _AttackCooldown;

}