using Godot;
using System;

public class SettingsController : Control
{
    private HSlider _MusicSlider;
    private CheckBox _SFXCheckbox;

    public override void _Ready()
    {
        base._Ready();

        _MusicSlider = GetNode<HSlider>("MusicSlider");
        _SFXCheckbox = GetNode<CheckBox>("SFXCheck");
    }

    public void SetupSettings()
    {
        _MusicSlider.Value = GetNode<GameController>("/root/GameController").GetCurrentSettings()._MusicLevel;
        _SFXCheckbox.Pressed = GetNode<GameController>("/root/GameController").GetCurrentSettings().SFXOn;
    }

    public void SaveSettings()
    {
        Settings newSettings = new Settings();

        // Get the audio values
        if(_MusicSlider != null && _SFXCheckbox != null)
        {
            newSettings._MusicLevel = (float)_MusicSlider.Value;
            newSettings.SFXOn = _SFXCheckbox.Pressed;
        }

        // Save & Load the new settings
        GetNode<GameController>("/root/GameController").SaveSettings(newSettings);
        GetNode<GameController>("/root/GameController").LoadSettings();
    }

    public void OnBackPressed()
    {
        this.Hide();
        GetNode<VBoxContainer>("/root/Node2D/Canvas/ColorRect/OptionsMenu").Show();
    }

    public void OnSavePressed()
    {
        SaveSettings();
    }
}

[Serializable]
public class Settings
{
    public float _MusicLevel;
    public bool SFXOn;
}
