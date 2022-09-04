using Godot;
using System;

public enum FacingDirection
{
    LEFT,
    RIGHT
}

public class PlayerController : CharacterController
{
    // === Movement Settings === //

    private bool _IsInventoryOpen = false;
    
    
    // === Components === //
    private RayCast2D _RaycastX;
    private RayCast2D _RaycastY;


    // === Attack Settings === //
    
    private FacingDirection _FacingDirection;           // Direction we are facing
    // Reference to the position of the sword anchor when the sprite is flipped
    [Export] private Vector2 _SwordAnchorPointFlipped;
    // Reference to the position of the sword when the sprite is not flipped
    [Export] private Vector2 _SwordAnchorPointNoFlip;

    public IInteractable _InteractableItem;                  // Reference to what item we can interact with

    // === Owner === //

    public override void _Ready()
    {
        base._Ready();

        // Get the ray cast properties
        _RaycastX = GetNode<RayCast2D>("RaycastX");
        _RaycastY = GetNode<RayCast2D>("RaycastY");
        
        // Setup the character
        _OwningCharacter = GetNode<GameController>("/root/GameController").GetPlayerCharacter();
        _OwningCharacter.SetOwningController(this);
        GetUI().UpdateHealthBar(_OwningCharacter.GetCurrentHealth());
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // Check for attack input & Attack
        if (Input.IsActionPressed("Attack"))
            Attack();
        
        if(Input.IsActionPressed("Test_Button"))
            _OwningCharacter.GetInventory().PrintInventory();

        if (Input.IsActionPressed("Inventory"))
        {
            ToggleInventory();
        }
        
        if(Input.IsActionPressed("Interact"))
            Interact();
        
    }

    private void ToggleInventory()
    {
        // Flip statemen

        if (_IsInventoryOpen)
        {
            GetNode<InventoryUI>("Canvas/InventoryRect").CloseInventory();
            _IsInventoryOpen = false;
        }
        else
        {
            
            GetNode<InventoryUI>("Canvas/InventoryRect").OpenInventory(_OwningCharacter.GetInventory());
            _IsInventoryOpen = true;
        }
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Movement(delta);
    }

    private void Movement(float delta)
    {
        var velocity = new Vector2();
        
        if (Input.IsActionPressed("Move_Up"))
            velocity.y -= 1;

        if (Input.IsActionPressed("Move_Down"))
            velocity.y += 1;

        if (Input.IsActionPressed("Move_Left"))
            velocity.x -= 1;

        if (Input.IsActionPressed("Move_Right"))
            velocity.x += 1;

        if (velocity.Length() > 0)
            velocity = velocity.Normalized() * _MovementSpeed;

        // Moves the player and checks for movement
        var collision = MoveAndCollide(velocity);  
        // Update the animations
        AnimationUpdate(velocity);
        // Set the direction the player is facing
        SetFacingDirection(velocity);
        
        // Update the direction the ray cast is facing
        UpdateRaycastPosition(velocity);
        

    }

    protected override void Attack()
    {
        base.Attack();
        // Don't perform method if we don't have the sword animation
        if (_SwordAnim == null || !_CanAttack)
            return;

        if (_FacingDirection == FacingDirection.RIGHT)
        {
            _SwordAnim.Play("Swing");
        } else if (_FacingDirection == FacingDirection.LEFT)
        {
            _SwordAnim.Play("SwingFlipped");
        }
        
        // TODO: Apply force to the enemy when they have been hit

        DetectAttackRaycast();
        _CanAttack = false;
    }
    

    protected override void AnimationUpdate(Vector2 vel)
    {
        base.AnimationUpdate(vel);
        
        // Return if we have no character or if the character is dead.
        if (_OwningCharacter != null && _OwningCharacter.IsAlive() == false)
            return;
        
        // Determine which animation to play
        if (vel == new Vector2())
        {
            _Anim.Play("Idle");
        }
        else
        {
            _Anim.Play("Walk");
        }
    }
    
    private void DetectAttackRaycast()
    {
        if (!_CanAttack)
            return;

        // Check which raycast is colliding and perform attack
        if (_RaycastX.IsColliding())
        {
            Node2D collidingNode = _RaycastX.GetCollider() as Node2D;
            if (collidingNode is CharacterController)
            {
                var enemyController = collidingNode as CharacterController;
                if (enemyController != null)
                {
                    AttackEnemy(enemyController);
                    if (_RaycastX.CastTo == new Vector2(20, 0))
                    {
                        var controller = enemyController as EnemyController;
                        controller.ApplyingForce = new Vector2(5, 0);
                    }
                    else
                    {
                        var controller = enemyController as EnemyController;
                        controller.ApplyingForce = new Vector2(-5, 0);
                    }
                }
            }
        } else if (_RaycastY.IsColliding())
        {
            Node2D collidingNode = _RaycastY.GetCollider() as Node2D;
            if (collidingNode is CharacterController)
            {
                var enemyController = collidingNode as CharacterController;
                if (enemyController != null)
                {
                    AttackEnemy(enemyController);
                    if (_RaycastY.CastTo == new Vector2(0, 20))
                    {
                        var controller = enemyController as EnemyController;
                        controller.ApplyingForce = new Vector2(0, 5);
                    }
                    else
                    {
                        var controller = enemyController as EnemyController;
                        controller.ApplyingForce = new Vector2(0, -5);
                    }
                }
            }
        }
    }

    private void AttackEnemy(CharacterController enemy)
    {
        if (enemy != null)
        {
            GD.Print("Hello World");
            enemy.GetOwningCharacter()?.TakeDamage(GetOwningCharacter().GetCurrentAP());
        }
    }

    private void Interact()
    {
        if (_InteractableItem != null)
        {
            if (_InteractableItem is ItemPickup)
            {
                var item = _InteractableItem as ItemPickup;
                if (_OwningCharacter.GetInventory().AddItem(item.OwningItem))
                    item.QueueFree();
            } else if (_InteractableItem is LootChest)
            {
                var chest = _InteractableItem as LootChest;
                chest?.OpenChest();
            } else if (_InteractableItem is LeverController)
            {
                var lever = _InteractableItem as LeverController;
                lever.OpenDoor();
            }
        }
    }

    private void SetFacingDirection(Vector2 vel)
    {
        if (vel.x > 0)
        {
            _Anim.FlipH = false;
            _FacingDirection = FacingDirection.RIGHT;
            _SwordSprite.Offset = _SwordAnchorPointNoFlip;

        } else if (vel.x < 0)
        {
            _Anim.FlipH = true;
            _FacingDirection = FacingDirection.LEFT;
            _SwordSprite.Offset = _SwordAnchorPointFlipped;
        }
    }

    private void UpdateRaycastPosition(Vector2 vel)
    {
        if (vel.y > 0)
        {
            _RaycastY.CastTo = new Vector2(0, 20);
        } else if (vel.y < 0)
        {
            _RaycastY.CastTo = new Vector2(0, -20);
        }

        if (vel.x > 0)
        {
            _RaycastX.CastTo = new Vector2(20, 0);
        } else if (vel.x < 0)
        {
            _RaycastX.CastTo = new Vector2(-20, 0);
        }
    }

    public UIController GetUI() => GetNode<UIController>("Canvas");
}
