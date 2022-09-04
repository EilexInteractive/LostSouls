using Godot;
using System;
using System.Security.Policy;

public class UIController : CanvasModulate
{
    private TextureProgress _HealthBar;
    private Label _PickupLabel;
    
    // === Message Details
    private TextureRect _MessageRect;
    private Timer _MessagePrintTimer;
    [Export] private float _MessageTime;                    // How much each character should be
    private string _Message;
    private string _CurrentMessage;
    private int _MessageIndex = 0;

    public override void _Ready()
    {
        base._Ready();

        // Get the required nodes
        _HealthBar = GetNode<TextureProgress>("HealthBar");
        _MessageRect = GetNode<TextureRect>("TextureRect");
        _PickupLabel = GetNode<Label>("PickupLabel");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // Update the timer
        if(_MessagePrintTimer != null)
            _MessagePrintTimer.UpdateTimer(delta);
    }

    public void UpdateHealthBar(float _CurrentHealth)
    {
        _HealthBar.Value = _CurrentHealth;
    }

    public void SetMessage(string message)
    {
        _MessageRect.Show();
        GetNode<RichTextLabel>("TextureRect/RichTextLabel").Text = "";
        _Message = message;
        _CurrentMessage = "";
        _MessageIndex = 0;
        _MessagePrintTimer = new Timer(_MessageTime, true, PrintMessage);
    }
    

    public void PrintMessage()
    {
        _CurrentMessage += _Message[_MessageIndex];
        _MessageIndex++;
        GetNode<RichTextLabel>("TextureRect/RichTextLabel").Text = _CurrentMessage;

        if (_CurrentMessage.Length() >= _Message.Length())
            _MessagePrintTimer = null;
    }

    public void TogglePickupLabel(bool toggle, string message = "")
    {
        if (toggle)
            _PickupLabel.Show();
        else
            _PickupLabel.Hide();

        if (message != "")
            _PickupLabel.Text = message;
    }

    public void HideMessageRect()
    {
        _MessageRect.Hide();
    }

}
