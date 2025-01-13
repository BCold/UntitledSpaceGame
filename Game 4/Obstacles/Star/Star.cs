using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Star : Area2D
{
    [Signal]
    public delegate void gravityAlteredEventHandler(Node2D body,bool status);
    
    [Export]
    private float _maxGrav = 250;
    public float MaxGrav{
        get{return _maxGrav;}
        set{_maxGrav = value;}
    }

    private float _gravStrength;
    public float GravStrength{
        get{return _gravStrength;}
        set{_gravStrength = value;}
    }

    private float _gravRadius;
    public float GravRadius{
        get{return _gravRadius;}
        set{_gravRadius = value;}
    }
    private Vector2 _gravDir;
    public Vector2 GravDir{
        get{return _gravDir;}
        set{_gravDir = value;}
    }

    private CircleShape2D gravityArea;

    public override void _Ready()
    {
        gravityArea = (CircleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
        GravRadius = gravityArea.Radius;
    }

    public override void _Process(double delta)
    {   
        // UpdateGravity();
    }

    internal void UpdateGravity(Node2D body){
        GravDir = (body.GlobalPosition - GlobalPosition).Normalized();
        float playerDistanceFromCenter = Mathf.Clamp(body.GlobalPosition.DistanceTo(GlobalPosition), 1, GravRadius - GravRadius / 100);
        GravStrength = (playerDistanceFromCenter / GravRadius * MaxGrav) - MaxGrav;
        //*** UNCOMMENT FOR DEBUG HELP ***\\
        // GD.Print($"GravDir: {GravDir}, playerDistanceFromCenter{playerDistanceFromCenter}, GravStrength: {GravStrength}");
    }

    private void OnBodyEntered(Node2D body){
        GD.Print($"{body.Name} entered Star area");
        EmitSignal("gravityAltered", body, true);
    }

    private void OnBodyExited(Node2D body){
        GD.Print($"{body.Name} exited Star area");
        EmitSignal("gravityAltered", body, false);
    }
}
