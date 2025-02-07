using Godot;
using System;
using System.Data;
using System.Security.Cryptography;

public partial class Mob : CharacterBody3D
{
    [Export] public int MinSpeed { get; set; }
    [Export] public int MaxSpeed { get; set; }

    [Signal] public delegate void SquashedEventHandler();
    [Signal] public delegate void ScreenExitedEventHandler();
    public override void _Ready()
    {
        InitConfig();
    }
    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }
    private void InitConfig()
    {
        GD.Print($"Init Mob");
        MinSpeed = ConfigLoader.GetMobConfigValue<int>("min_speed");
        MaxSpeed = ConfigLoader.GetMobConfigValue<int>("max_speed");
        GD.Print($"MinSpeed:{MinSpeed}");
        GD.Print($"MaxSpeed:{MaxSpeed}");
    }
    public void Initialize(Vector3 startPosition, Vector3 playerPositon)
    {
        LookAtFromPosition(startPosition, playerPositon, Vector3.Up);
        float randomAngle = (float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0);
        RotateY(randomAngle);

        int randomSpeed = GD.RandRange(MinSpeed, MaxSpeed);
        GD.Print("min speed: ",MinSpeed);
        GD.Print("max speed: ",MaxSpeed);
        GD.Print("random speed: ",randomSpeed);

        Velocity = Vector3.Forward * randomSpeed;
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
        
        GD.Print("x:",Velocity.X) ;
        GD.Print("y:",Velocity.Y); 
        GD.Print("z:",Velocity.Z); 
    }
    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
        GD.Print("Mob Squash!");
    }
    private void OnVisibilityNotifierScreenExited()
    {
        EmitSignal(SignalName.ScreenExited);
        QueueFree();
        GD.Print("Mob Exited Screen!");
    }
}
