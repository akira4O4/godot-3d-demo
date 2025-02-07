using Godot;
using System;
using System.Text.Json;
public partial class Main : Node3D
{
    [Export] public PackedScene MobScene { get; set; }

    private int _maxMob;
    private int _currMobCount = 0;
    public override void _Ready()
    {
        InitConfig();
        GetNode<Control>("UserInferface/Retry").Hide();
    }
    private void InitConfig()
    {
        GD.Print("Init System");
        _maxMob = ConfigLoader.GetSystemConfigValue<int>("max_mob");
        GD.Print($"maxMob:{_maxMob}");

    }
    private void OnPlayerHit()
    {
        GD.Print("Mob Timer Stop");
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Control>("UserInferface/Retry").Show();
    }

    private void OnMobTimerTimeout()
    {
        if (_currMobCount < _maxMob)
        {
            Mob mob = MobScene.Instantiate<Mob>();
            var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
            mobSpawnLocation.ProgressRatio = GD.Randf();
            Vector3 playerPosition = GetNode<Player>("Player").Position;

            mob.Initialize(mobSpawnLocation.Position, playerPosition);
            AddChild(mob);

            mob.Squashed += GetNode<ScoreLabel>("UserInferface/ScoreLabel").OnMobSquashed;
            mob.Squashed += KillMob;
            mob.ScreenExited += KillMob;

            _currMobCount += 1;
        }
    }

    public void KillMob()
    {
        if (_currMobCount != 0)
        {
            _currMobCount -= 1;
            GD.Print($"Killed The Mob,Curr Mob Size: {_currMobCount}");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("enter") && GetNode<Control>("UserInferface/Retry").Visible)
        {
            GetTree().ReloadCurrentScene();
        }
    }
}

