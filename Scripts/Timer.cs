using Godot;
using System;

[Serializable]
public class Timer
{
    private float _TimerLength;                 
    private float _CurrentTime;
    private bool _Loop;
    private Action _OnCompleteAction;
    private bool _IsActive;

    public Timer(float length, bool loop, Action onComplete, bool isActive = true)
    {
        _TimerLength = length;
        _Loop = loop;
        _OnCompleteAction = onComplete;
        _IsActive = isActive;
    }

    public void StartTimer()
    {
        _IsActive = true;
    }

    public void PauseTimer()
    {
        _IsActive = false;
    }

    public void RestartTimer()
    {
        _CurrentTime = 0;
        _IsActive = true;
    }

    public void UpdateTimer(float delta)
    {
        if (_IsActive == false)
            return;

        _CurrentTime += 1 * delta;
        if(_CurrentTime > _TimerLength)
            TimerComplete();
            
    }

    private void TimerComplete()
    {
        if (_OnCompleteAction != null)
            _OnCompleteAction();

        if (_Loop)
            RestartTimer();
    }
    
    
}