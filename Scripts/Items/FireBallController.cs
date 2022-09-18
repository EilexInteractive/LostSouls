using Godot;
using System;

public class FireBallController : Area2D
{

    [Export] private float _MovementSpeed = 10;
    [Export] private float FireballDamage = 35;
    [Export] private float DestroyTime = 3.0f;                      // How long before the object leaves the scene
 
    private Timer _destroyTimer;

    public override void _Ready()
    {
        base._Ready();
        _destroyTimer = new Timer(DestroyTime, false, Destroy);

        if(GetNode<GameController>("/root/GameController").GetCurrentSettings().SFXOn)
        {
            GetNode<AudioStreamPlayer2D>("SoundEffect").Play();
        }
        
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Process(float delta)
    {
        Position += new Vector2(_MovementSpeed, 0).Rotated(Rotation);

        if(_destroyTimer != null)
            _destroyTimer.UpdateTimer(delta);
    }

        public void OnFireballHit(Node body)
    {
        if(body is PlayerController)
        {
            var player = body as PlayerController;
            player.GetOwningCharacter().TakeDamage(FireballDamage);
        } else 
        {
            GD.Print("Hit Something");
        }


        QueueFree();
    }

    public void Destroy() => QueueFree();
    public void SetFireDamage(float damage) => FireballDamage = damage;
}
