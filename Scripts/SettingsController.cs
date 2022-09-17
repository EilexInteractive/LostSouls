using Godot;
using System;

public class SettingsController : Control
{
    private HSlider _MusicSlider;
    private HSlider _SFXSlider;

    public override void _Ready()
    {
        base._Ready();

        _MusicSlider = GetNode<HSlider>("MusicSlider");
        _SFXSlider = GetNode<HSlider>("SFXSlider");
    }

    public void SaveSettings()
    {
        Settings newSettings = new Settings();

        // Get the audio values
        if(_MusicSlider != null && _SFXSlider != null)
        {
            newSettings._MusicLevel = (float)_MusicSlider.Value;
            newSettings._SFXLevel = (float)_SFXSlider.Value;
        }

        // Save & Load the new settings
        GetNode<GameController>("/root/GameController").SaveSettings(newSettings);
        GetNode<GameController>("/root/GameController").LoadSettings();
    }

    public void OnBackPressed()
    {
        this.Hide();
        GetNode<Node2D>("/root/Main/Canvas/ColorRect/OptionsMenu").Show();
    }
}

[Serializable]
public class Settings
{
    public float _MusicLevel;
    public float _SFXLevel;
}
