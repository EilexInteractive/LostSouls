using Godot;
using System;



public class PlayerController : CharacterController
{
    // === Movement Settings === //

    private bool _IsInventoryOpen = false;
    public bool IsInDialog;                // If the dialog menu is open


    // === Components === //
    private RayCast2D _RaycastX;
    private RayCast2D _RaycastY;


    // === Attack Settings === //
    
    
    // Reference to the position of the sword anchor when the sprite is flipped
    [Export] private Vector2 _SwordAnchorPointFlipped;
    // Reference to the position of the sword when the sprite is not flipped
    [Export] private Vector2 _SwordAnchorPointNoFlip;

    public IInteractable _InteractableItem;                  // Reference to what item we can interact with

    // === Owner === //

    private bool _DeathScreenPlayed = false;
    private bool _IsShowingFriendlyInteract = false;

    private event Action _DialogEvent; 

    



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
        GetUI().UpdateXP(_OwningCharacter.GetCurrentXP(), _OwningCharacter.GetMaxXP(), _OwningCharacter.GetCurrentLevel());
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        // Check for attack input & Attack
        if (Input.IsActionJustPressed("Attack"))
            Attack();
        
        DetectFriendlyInteraction();

        if (Input.IsActionJustPressed("Inventory"))
        {
            ToggleInventory();
        }

        if (Input.IsActionJustPressed("Dialog"))
        {
            if (IsInDialog)
            {
                if (GetUI().GetMessage() != "")
                {
                    if (GetUI().IsMessageFinished())
                    {
                        GetUI().HideMessageRect();
                        _DialogEvent?.Invoke();
                    }
                    else
                    {
                        GetUI().FinishMessage();
                    }
                }
            }
        }
        
        
        if(Input.IsActionJustPressed("Interact"))
            Interact();

        if(Input.IsActionJustPressed("Pause"))
        {
            GetNode<CanvasModulate>("Canvas").Hide();
            GetNode<PauseMenu>("PauseMenu").Show();
            GetTree().Paused = true;
        }
        
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
        {
            velocity.y -= 1;
            _FacingDirection = FacingDirection.UP;
        }
            

        if (Input.IsActionPressed("Move_Down"))
            {
                velocity.y += 1;
                _FacingDirection = FacingDirection.DOWN;
            }

        if (Input.IsActionPressed("Move_Left"))
        {
            velocity.x -= 1;
            _FacingDirection = FacingDirection.LEFT;
        }

        if (Input.IsActionPressed("Move_Right"))
        {
            velocity.x += 1;
            _FacingDirection = FacingDirection.RIGHT;
        }
            

        if (velocity.Length() > 0)
            velocity = velocity.Normalized() * _MovementSpeed;

        // Moves the player and checks for movement
        var collision = MoveAndCollide(velocity);  
        // Update the animations
        AnimationUpdate(velocity);
        
        // Update the direction the ray cast is facing
        UpdateRaycastPosition(velocity);
        

    }

    private void DetectFriendlyInteraction()
    {
        if (_RaycastX.IsColliding())
        {
            if (_RaycastX.GetCollider() is Friendly)
            {
                _InteractableItem = _RaycastX.GetCollider() as Friendly;
                GetUI().TogglePickupLabel(true, "Interact");
            }
        } else if (_RaycastY.IsColliding())
        {
            if (_RaycastY.GetCollider() is Friendly)
            {
                _InteractableItem = _RaycastY.GetCollider() as Friendly;
                GetUI().TogglePickupLabel(true, "Interact");
            }
        }
        else
        {
            if(_IsShowingFriendlyInteract)
                GetUI().TogglePickupLabel(false);
        }
    }

    protected override void Attack()
    {
        base.Attack();

        _IsAttacking = true;

        // Don't attack if our inventory is open
        if(_IsInventoryOpen)
            return;


        // Trigger sword sound
        if(_SwordAudio != null)
            _SwordAudio.Play();

        switch(_FacingDirection)
        {
            case FacingDirection.UP:
                _Anim.Play("AttackUp");
                break;
            case FacingDirection.RIGHT:
                _Anim.Play("AttackRight");
                break;
            case FacingDirection.DOWN:
                _Anim.Play("AttackDown");
                break;
            case FacingDirection.LEFT:
                _Anim.Play("AttackLeft");
                break;

        }
        

        DetectAttackRaycast();
        _CanAttack = false;
    }
    

    protected override void AnimationUpdate(Vector2 vel)
    {
        base.AnimationUpdate(vel);
        
        // Return if we have no character or if the character is dead.
        if (_OwningCharacter != null && _OwningCharacter.IsAlive() == false)
            return;

        if(_IsAttacking)
            return;
        
        // Determine which animation to play
        if (vel == new Vector2())
        {
            switch(_FacingDirection)
            {
                case FacingDirection.LEFT:
                    _Anim.Play("IdleLeft");
                    break;
                case FacingDirection.RIGHT:
                    _Anim.Play("IdleRight");
                    break;
                case FacingDirection.UP:
                    _Anim.Play("IdleUp");
                    break;
                case FacingDirection.DOWN:
                    _Anim.Play("IdleDown");
                    break;
            }
        }
        else
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
                if (collidingNode is EnemyController)
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
    }

    private void AttackEnemy(CharacterController enemy)
    {
        if (enemy != null)
        {
            if (enemy is Friendly)
                return;

            var dp = GetOwningCharacter().GetCurrentAP() * GetOwningCharacter().GetInventory().GetEquippedWeapon().GenerateDP();
            
            enemy.GetOwningCharacter()?.TakeDamage(dp);
            _OwningCharacter.AddXP(GenerateXP(enemy.GetOwningCharacter().GetCurrentLevel()));
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
            } else if (_InteractableItem is Friendly)
            {
                var friend = _InteractableItem as Friendly;
                friend.Interact();
            }
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

    public override void TriggerDeathAnim()
    {
        base.TriggerDeathAnim();

        if (!_DeathScreenPlayed)
        {
            GetNode<ColorRect>("Canvas/DeathScreen").Show();
            GetNode<Button>("Canvas/DeathScreen/Button").Disabled = false;          // Enable the respawn button
            GetNode<AnimationPlayer>("Canvas/DeathScreen/AnimationPlayer").Play("DeathScreenIn");
        }
            

        _DeathScreenPlayed = true;
    }

    public override void SetEquippedItem(Weapon weapon)
    {
        base.SetEquippedItem(weapon);

        
        string animationPath = "res://Animations/PlayerCharacters/";
        if(_OwningCharacter.GetInventory().GetEquippedArmour() == null)
        {
            animationPath += "NoArmour/";
        }

        switch(weapon.GetWeaponType())
        {
            case WeaponType.LONG_SWORD:
                animationPath += "Player_Sword.tres";
                break;
            case WeaponType.AXE:
                animationPath += "Player_Axe.tres";
                break;
            case WeaponType.HALBERD:
                animationPath += "Player_Halbred.tres";
                break;
            case WeaponType.WARHAMMER:
                animationPath += "Player_Warhammer.tres";
                break;
            case WeaponType.WAR_AXE:
                animationPath += "Player_WarAxe.tres";
                break;
            default:
                animationPath += "Player_Sword.tres";
                GD.PrintErr("No animation for weapon type");
                break;
        }

        _Anim.Frames = GD.Load<SpriteFrames>(animationPath);            // Load the animation
    }

    public void OnAnimFinished()
    {
        if(_IsAttacking)
        {
            _IsAttacking = false;
        }
    }

    public void OnRespawnPressed()
    {
        _OwningCharacter.Respawn();
        GetNode<ColorRect>("Canvas/DeathScreen").Hide();
        GetNode<GameController>("/root/GameController").LoadGame();
        GetOwningCharacter().IncreaseHealth(100);           // Set the health back to full bar
    }

    private float GenerateXP(int enemyLevel)
    {
        return (float)GD.RandRange(1, 6) * enemyLevel;
    }

    public UIController GetUI() => GetNode<UIController>("Canvas");
    public void SetDialogEvent(Action dialogEvent) => _DialogEvent = dialogEvent;
}


