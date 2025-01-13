using Godot;

public partial class Projectile : CharacterBody2D
{
    [Signal]
    public delegate void playerHitEventHandler(GodotObject body);

    [Export(hint: PropertyHint.Range, hintString: "-1000, 1000")]
    private float _speed = 100;
    public float Speed{
        get{return _speed;}
        set{_speed = value;}
    }
    [Export]
    private float lifetime;
    private Node2D parent; // Stores the Node that spawned this Projectile. Not currently in use.
    private Timer lifeTimer; // Stores the lifeTimer Timer

    public override void _Ready()
    {
        lifeTimer = GetNode<Timer>("LifeTimer");
        lifeTimer.WaitTime = lifetime;
        lifeTimer.Start();
        // star.gravityAltered += OnGravityAltered; // This is how you manually connect a signal using events in C#
        Velocity = Transform.X * Speed;
    }

    public override void _Process(double delta)
    {   
    // Gravity effects are WORK-IN-PROGRESS, will likely need iterated upon.
       KinematicCollision2D collision =  MoveAndCollide(Velocity * (float)delta);
       if (collision != null){
        GD.Print($"Collision with {collision.GetCollider()}");
        if (collision.GetCollider() is Player){
                EmitSignal("playerHit", collision.GetCollider());
                QueueFree();
        }
        else if (collision.GetCollider() is BounceTest ){
            Velocity = Velocity.Bounce(collision.GetNormal());
            Rotate(-Rotation);
        }
       }
    }

    private void LifeTimeout(){
        QueueFree();
    }

    // private void OnGravityAltered(Node2D body, bool status){
    //     if (body is Projectile){
    //         activeGravity = status;
    //         GD.Print($" {body.Name} projectile affected by gravity: {status}");
    //     }
    // }
}
