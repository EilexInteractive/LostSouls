using Godot;
using System;
using System.Collections.Generic;

public class MainSceneController : SceneController
{
    private Timer StartCombatTimer;
    private Timer FightOverTimer;               // Timer to run once the fight is over

    private EnemyController _Enemy;
    private PlayerController _Player;

    private bool _InitialDialogComplete = false;
    private bool _PostFightDialog = false;              // If we have displayed the post fight dialog
    private RoomData _RoomSaveData;

    [Export] private PackedScene _EnemySpawn;
    private Timer _ChangeSeenTimer;
    
    
    public override void _Ready()
    {
        base._Ready();
        
        // Spawn the new enemy to this scene
        var enemyInstance = _EnemySpawn.Instance();
        AddChild(enemyInstance);
        var enemyNode = enemyInstance as EnemyController;
        enemyNode.Position = GetNode<Node2D>("SpawnPoint").Position;
        _Enemy = enemyNode;
        
        LoadRoomData();
        
        _Player = GetNode<PlayerController>("Player");

        
        

        if (!_InitialDialogComplete)
        {
            // Setup a message to display on screen
            _Player.GetUI()?.SetMessage("We have awoken in a dark dungeon unaware of what has happened. The demon looks angry.... Good thing we have a sword");
            StartCombatTimer = new Timer(7.0f, false, StartCombat);             // Setup the new timer in 5 seconds
        }

        _ChangeSeenTimer = new Timer(5.0f, false, AllowSceneChange);

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        if(StartCombatTimer != null)
            StartCombatTimer.UpdateTimer(delta);
        

        if (!_Enemy.GetOwningCharacter().IsAlive() && !_PostFightDialog)
        {
            _PostFightDialog = true;
            _Player.GetUI()?.SetMessage("Where am i?? Maybe this door will have more answers");
        }

        
        if(_ChangeSeenTimer != null)
            _ChangeSeenTimer.UpdateTimer(delta);
        


    }

    public void AllowSceneChange()
    {
        GetNode<ChangeScene>("ChangeScene").CanChangeScene = true;
    }

    private void StartCombat()
    {
        _InitialDialogComplete = true;
        _Enemy.CanMove = true;
        _Player.CanMove = true;
        StartCombatTimer = null;                // Clear the timer once combat has started
    }

    public override void SaveRoom()
    {
        List<CharacterSaveData> characters = new List<CharacterSaveData>();             // Create a list for all the characters
        Character character = GetNode<CharacterController>("Character").GetOwningCharacter();
        Vector2 pos = GetNode<CharacterController>("Character").Position;
        characters.Add(new CharacterSaveData(character, pos));
        EntranceRoom room = new EntranceRoom(_Enemy.Position, characters);               // Create the room data
        GetNode<GameController>("/root/GameController").SaveRoomData(room);           // Save the room
    }

    public override void LoadRoomData()
    {
        base.LoadRoomData();
        if (_SaveData != null)
        {
            var roomData = _RoomSaveData as EntranceRoom;
            if (roomData.DialogComplete)
            {
                _InitialDialogComplete = true;
                _PostFightDialog = true;
            }

            CharacterSaveData enemy = _RoomSaveData.CharactersInRoom[0];
            GetNode<CharacterController>("Character").SetOwningCharacter(enemy.CharacterRef, false);
            GetNode<CharacterController>("Character").Position = enemy.Position;
            GetNode<ChangeScene>("ChangeScene").CanChangeScene = false;
            GetNode<CharacterController>("Player").Position = GetNode<Node2D>("ReturnPoint").Position;
        }
    }
}

[Serializable]
public class EntranceRoom : RoomData
{
    public bool DialogComplete;
    public Vector2 enemyPos;
    public EntranceRoom(Vector2 pos, List<CharacterSaveData> characters, bool dialogComplete = true) : base("Main", characters)
    {
        enemyPos = pos;
        DialogComplete = dialogComplete;
    }
}