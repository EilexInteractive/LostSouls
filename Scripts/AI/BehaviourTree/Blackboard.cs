using Godot;
using System;
using System.Collections.Generic;

public class Blackboard
{
    private List<BlackboardBase> _Blackboard = new List<BlackboardBase>();

    public int GetValueAsInt(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardInt;
                return result.Value;
            }
        }

        return 0;
    }

    public float GetValueAsFloat(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardFloat;
                return result.Value;
            }
        }

        return 0;
    }

    public bool GetValueAsBool(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardBool;
                return result.Value;
            }
        }

        return false;
    }

    public Vector2 GetValueAsVector2(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardVector2;
                return result.Value;
            }
        }

        return new Vector2();
    }

    public Node2D GetValueAsNode2D(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardNode2D;
                return result.Value;
            }
        }

        return null;
    }

    public EnemyState GetValueAsEnemyState(string keyName)
    {
        foreach(var key in _Blackboard)
        {
            if(key.Key == keyName)
            {
                var result = key as BlackboardEnemyState;
                return result.Value;

            }
        }

        return EnemyState.NULL;
    }

    public void SetValueAsInt(string keyName, int value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardInt;
                result.Value = value;
                return;
            }
        }

        BlackboardInt NewKey = new BlackboardInt(keyName, value);
        _Blackboard.Add(NewKey);
    }

    public void SetValueAsFloat(string keyName, float value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardFloat;
                result.Value = value;
                return;
            }
        }

        BlackboardFloat NewKey = new BlackboardFloat(keyName, value);
        _Blackboard.Add(NewKey);
    }

    public void SetValueAsBool(string keyName, bool value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardBool;
                result.Value = value;
                return;
            }
        }

        BlackboardBool NewKey = new BlackboardBool(keyName, value);
        _Blackboard.Add(NewKey);
    }

    public void SetValueAsVector2(string keyName, Vector2 value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardVector2;
                result.Value = value;
                return;
            }
        }

        BlackboardVector2 NewKey = new BlackboardVector2(keyName, value);
        _Blackboard.Add(NewKey);
    }

    public void SetValueAsNode2D(string keyName, Node2D value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardNode2D;
                result.Value = value;
                return;
            }
        }

        BlackboardNode2D NewKey = new BlackboardNode2D(keyName, value);
        _Blackboard.Add(NewKey);
    }

    public void SetValueAsEnemyState(string keyName, EnemyState value)
    {
        foreach(var blackboardKey in _Blackboard)
        {
            if(blackboardKey.Key == keyName)
            {
                var result = blackboardKey as BlackboardEnemyState;
                result.Value = value;
                return;
            }
        }

        BlackboardEnemyState NewKey = new BlackboardEnemyState(keyName, value);
        _Blackboard.Add(NewKey);
    }
}

public class BlackboardBase
{
    public string Key;

    public BlackboardBase(string keyName)
    {
        Key = keyName;
    }
}

public class BlackboardInt : BlackboardBase
{
    public int Value;

    public BlackboardInt(string keyName, int value) : base(keyName)
    {
        Value = value;
    }
}

public class BlackboardFloat : BlackboardBase
{
    public float Value;
    
    public BlackboardFloat(string keyName, float value) : base(keyName)
    {
        Value = value;
    }
}

public class BlackboardVector2 : BlackboardBase
{
    public Vector2 Value;
    public BlackboardVector2(string keyName, Vector2 value) : base(keyName)
    {
        Value = value;
    }
}

public class BlackboardBool : BlackboardBase
{
    public bool Value;

    public BlackboardBool(string keyName, bool value) : base(keyName)
    {
        Value = value;
    }

}

public class BlackboardNode2D : BlackboardBase
{
    public Node2D Value;
    public BlackboardNode2D(string keyName, Node2D value) : base(keyName)
    {
        Value = value;
    }
}

public class BlackboardEnemyState : BlackboardBase
{
    public EnemyState Value;
    public BlackboardEnemyState(string keyName, EnemyState value) : base(keyName)
    {
        Value = value;
    }
}