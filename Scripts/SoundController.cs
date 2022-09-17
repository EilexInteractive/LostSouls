using Godot;
using System;
using System.Collections.Generic;
public class SoundController : Node
{
    private AudioStreamPlayer2D _SoundNode;
    private List<AudioStream> _SoundTrack = new List<AudioStream>();

    public override void _Ready()
    {
        base._Ready();

        _SoundTrack = new List<AudioStream>()
        {
            GD.Load<AudioStream>("res://SFX/Music/Dungeon 01.mp3")
        };

        _SoundNode = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");                   // Get the sound node
        PlayTrack(0);                   // Play the first track on load
        
    }

    /// <summary>
    /// Plays the track at the index passed in
    /// </summary>
    /// <param name="trackIndex">Track Index</param>
    public void PlayTrack(int trackIndex)
    {
        _SoundNode.Stream = _SoundTrack[0];
        _SoundNode.Play();
    }

    public void PauseTrack() => _SoundNode.StreamPaused = true;
    public void PlayTrack() => _SoundNode.StreamPaused = false;
}
