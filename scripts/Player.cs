using System;
using Godot;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void HitEventHandler();

    [Signal] public delegate void ScreenExitedEventHandler();

    [Export]
    public int Speed { get; set; }
    [Export]
    public int FallAcceleration { get; set; }
    [Export]
    public int JumpImpulse { get; set; }
    [Export]
    public int BounceImpulse { get; set; }

    private int _maxJumps;
    private int _jumpCount;

    private Vector3 _targetVelocity = Vector3.Zero;
    public override void _Ready()
    {
        InitConfig();
    }
    private void InitConfig()
    {
        GD.Print($"Init Player");
        Speed = ConfigLoader.GetPlayerConfigValue<int>("speed");
        FallAcceleration = ConfigLoader.GetPlayerConfigValue<int>("fall_acceleration");
        JumpImpulse = ConfigLoader.GetPlayerConfigValue<int>("jump_impulse");
        BounceImpulse = ConfigLoader.GetPlayerConfigValue<int>("bounce_impulse");
        _maxJumps = ConfigLoader.GetPlayerConfigValue<int>("max_jumps");
        GD.Print($"Speed:{Speed}");
        GD.Print($"FallAcceleration:{FallAcceleration}");
        GD.Print($"JumpImpulse:{JumpImpulse}");
        GD.Print($"BounceImpulse:{BounceImpulse}");
        GD.Print($"MaxJumps:{_maxJumps}");
    }
    public override void _PhysicsProcess(double delta)
    {
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        // We check for each move input and update the direction accordingly.
        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
        }

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;


        if (IsOnFloor())
        {
            _jumpCount = 0;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            if (_jumpCount < _maxJumps)
            {
                _targetVelocity.Y = JumpImpulse;
                _jumpCount++;
            }
        }
        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            KinematicCollision3D collision = GetSlideCollision(index);

            if (collision.GetCollider() is Mob mob)
            {
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                    break;
                }
            }
        }

        // Moving the Character
        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    private void Die()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
        GD.Print("Your Die");
    }

    private void OnMobDetectorBodyEntered(Node3D body)
    {
        Die();
        EmitSignal(SignalName.ScreenExited);
    }
    private void OnVisibilityNotifierScreenExited()
    {
        // GetNode<Timer>("MobTimer").Stop();
        // GetNode<Control>("UserInferface/Retry").Show();
        GD.Print("You Free!");
    }
}