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
}
