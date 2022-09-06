using Godot;
using System;

public class SpikeTrap : Area2D
{
    private bool _IsUp = false;                 // if the spikes have been triggered up
    private AnimatedSprite _Anim;               // Reference to the animator
    private Timer _SpikeUpTimer;                // How long before the spikes will go up
    private Timer _SpikeDownTimer;              // How long before the spikes will go down

    [Export] private float _DamagePoints = 30;       // How much damage the traps will take from the player

    private bool _IsPlayerOnTrap;               // If the player is on the trap

    public override void _Ready()
    {
        base._Ready();
        _Anim = GetNode<AnimatedSprite>("AnimatedSprite");
        PlayDownAnimation();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        if(_SpikeDownTimer != null)
            _SpikeDownTimer.UpdateTimer(delta);

        if (_SpikeUpTimer != null)
            _SpikeUpTimer.UpdateTimer(delta);
    }

    public void PlayUpAnimation()
    {
        _IsUp = true;
        _SpikeUpTimer = null;
        _Anim.Play("Up");
        _SpikeDownTimer = new Timer(((float)GD.RandRange(1, 4)), false, PlayDownAnimation);

        if (_IsPlayerOnTrap)
        {
           GetNode<GameController>("/root/GameController").GetPlayerCharacter().TakeDamage(_DamagePoints);
        }
    }

    public void PlayDownAnimation()
    {
        _IsUp = false;
        _SpikeDownTimer = null;
        _Anim.Play("Down");
        _SpikeUpTimer = new Timer(((float)GD.RandRange(1, 3)), false, PlayUpAnimation);
    }

    public void OnSpikeEnter(Node body)
    {
        if (body is PlayerController)
        {
            _IsPlayerOnTrap = true;
        }
    }

    public void OnSpikeExit(Node body)
    {
        if (body is PlayerController)
        {
            _IsPlayerOnTrap = false;
        }
    }
}
