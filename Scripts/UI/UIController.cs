using Godot;
using System;
using System.Security.Policy;

public class UIController : Control
{
    private TextureProgress _HealthBar;
    private TextureProgress _XPBar;
    private Label _PickupContainer;

    // === Message Details
    private TextureRect _MessageRect;                       // Reference to the message container
    private Timer _MessagePrintTimer;                       // Timer that will add a letter to the message
    [Export] private float _MessageTime;                    // How much each character should be
    private string _Message;                                // Reference to the full message
    private string _CurrentMessage;                         // Part of the message that is currently being displayed
    private int _MessageIndex = 0;                          // Character that we are up to in the message
    private bool _IsMessageFinished = false;                // If the message has finished printing

    public event Action FollowUpMessageEvent;

    public override void _Ready()
    {
        base._Ready();

        // Get the required nodes
        _HealthBar = GetNode<TextureProgress>("CanvasLayer/HealthBar");
        _MessageRect = GetNode<TextureRect>("CanvasLayer/TextureRect");
        _PickupContainer = GetNode<Label>("CanvasLayer/PickupLabel");
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

    public void UpdateXP(float CurrentXP, float MaxXP, int CurrentLevel)
    {
        // Set the text to be our current level
        GetNode<Label>("CanvasLayer/XPBar/Label2").Text = CurrentLevel.ToString();
        // Setup values
        GetNode<TextureProgress>("CanvasLayer/XPBar").MaxValue = MaxXP;
        GetNode<TextureProgress>("CanvasLayer/XPBar").Value = CurrentXP;
    }

    public void SetMessage(string message)
    {
        _MessageRect.Show();
        GetNode<RichTextLabel>("CanvasLayer/TextureRect/RichTextLabel").Text = "";
        _Message = message;
        _CurrentMessage = "";
        _MessageIndex = 0;
        _MessagePrintTimer = new Timer(_MessageTime, true, PrintMessage);
        var player = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController();
        if (player != null)
        {
            player.CanMove = false;
            var pc = player as PlayerController;;
            pc.IsInDialog = true;
        }
            
    }

    public void FinishMessage()
    {
        _MessagePrintTimer = null;
        _CurrentMessage = _Message;
        GetNode<RichTextLabel>("CanvasLayer/TextureRect/RichTextLabel").Text = _CurrentMessage;
        _IsMessageFinished = true;
    }
    

    public void PrintMessage()
    {
        _CurrentMessage += _Message[_MessageIndex];
        _MessageIndex++;
        GetNode<RichTextLabel>("CanvasLayer/TextureRect/RichTextLabel").Text = _CurrentMessage;

        if (_CurrentMessage.Length() >= _Message.Length())
        {
            _MessagePrintTimer = null;
            _IsMessageFinished = true;
        }
            
    }
    
    public void HideMessageRect()
    {
        _MessageRect.Hide();
        var player = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController();
        player.CanMove = true;
        var pc = player as PlayerController;
        pc.IsInDialog = false;

        var friend = GetNode<Friendly>("Friendly");
        if(friend != null)
            friend.GetDialogController()?.CloseDialog();

        _IsMessageFinished = false;

        FollowUpMessageEvent?.Invoke();
        FollowUpMessageEvent = null;

        
    }

    public void TogglePickupLabel(bool toggle, string message = "")
    {
        if (toggle && message != "")
        {
            GetNode<Label>("CanvasLayer/PickupLabel").Show();
            GetNode<Label>("CanvasLayer/PickupLabel").Text = message;
        }
        else
        {
            GetNode<Label>("CanvasLayer/PickupLabel").Hide();
        }
    }

    public void ToggleItemInfo(bool toggle, Item item = null)
    {
        if (item != null)
        {
            if (toggle)
            {
                GetNode<ColorRect>("CanvasLayer/InteractableInfo").Show();
                GetNode<Label>("CanvasLayer/InteractableInfo/ItemName").Text = item.GetItemName();
                GetNode<Label>("CanvasLayer/InteractableInfo/ItemDescription").Text = item.GetItemDesc();
            }
            else
            {
                GetNode<ColorRect>("CanvasLayer/InteractableInfo").Hide();
            }
        }
        else
        {
            GetNode<ColorRect>("CanvasLayer/InteractableInfo").Hide();
        }
    }

    

    public bool IsMessageFinished() => _IsMessageFinished;
    public string GetMessage() => _Message;

}
