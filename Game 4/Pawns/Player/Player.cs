using System.Runtime.CompilerServices;
using Godot;
public partial class Player : CharacterBody2D
{


     // Export Variables
    [ExportGroup("Movement Properties")]
    [Export(hint: PropertyHint.Range, hintString: "-90, 90")]
    float rotationStrength = 5; // Determines the magnitude of the rotation input.
    [Export(hint: PropertyHint.Range, hintString: "-1000, 1000")]
    float thrustStrength = 100; // Determines the maximum thrust magnitude for the thrust input.

    [Export(hint: PropertyHint.Range, hintString: "-2000, 2000")]
    float decayStrength = 200; // Used to determine how long it takes for the player ship to come to a stop.

     [Export(hint: PropertyHint.Range, hintString: "-1000, 1000")]
     float drag = 10; // Used to determine how long it takes for the player ship to reach maximum thrust.

    [Export(hint: PropertyHint.Range, hintString: "-1, 1")]
     private float playerFov = 0.45f; // Represents the player's forward facing angle. Note: -1 is equal to 180 degrees, 0 is equal to 90 degrees, and 1 is equal to 0 degrees.
    
    [Export]
    private float mpSyncWeight;

    // Other Player related variables.
    private float _rotationDirection; // Used to store rotation input value.

    private Vector2 syncPos = new();
    private float syncRotation = 0;

    public override void _Ready()
    {
        GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
    }

    PackedScene projectile = GD.Load<PackedScene>("res://Pawns/Player/Projectile/Projectile.tscn"); // Projectile scene to be instantiated upon "shoot" input.
    
    public override void _Process(double delta)
    {
        if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {
            GetInput(delta);
            Rotate(_rotationDirection * rotationStrength * (float)delta);
            MoveAndCollide(Velocity * (float)delta);
        }
    }

// Responsible for grabbing user input and determining what to do with it.
    public void GetInput(double delta){
        // Set rotation direction based on player input
        _rotationDirection = Input.GetAxis("rotate_left", "rotate_right");

        if (Input.IsActionPressed("thrust")){
        // As long as the player is holding "thrust" then step velocity upward until hitting thrustStrength and then maintain it.
            Velocity = Velocity.MoveToward(Transform.X * thrustStrength, thrustStrength / drag);
        }
        else{
            // Once "thrust" input stops then step velocity downward until hitting 0.
            Velocity = Velocity.MoveToward(Transform.X * 0, thrustStrength / decayStrength);
        }

        // Shoot
        if (Input.IsActionJustPressed("shoot")){
            // GD.Print("Bang!");
            Rpc("SpawnProjectile");
        }

    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void SpawnProjectile(){
        Projectile newProjectile = (Projectile)projectile.Instantiate();
        GetParent().AddChild(newProjectile);
        newProjectile.GlobalPosition = GetNode<Node2D>("ProjectileSpawner").GlobalPosition;
        newProjectile.RotationDegrees = GetNode<Node2D>("ProjectileSpawner").RotationDegrees;
        newProjectile.Velocity = Transform.X * newProjectile.Speed;
        newProjectile.Rotate(Rotation);
        // GetParent().AddChild(newProjectile);
    }


    
    private void HitCheck(GodotObject body){
        GD.Print($"");
    }
}

/////********************   OLD & UNDER EVALUATION   ********************\\\\\
    //      // REQUIREMENT OF STAR EXPORT AND FOLLOWING VARIABLES UNDER EVALUATION.
    //      [Export] private Star star; // Stores the star node, assigned within the ready function.
    //      [Export(hint: PropertyHint.Range, hintString: "-1000, 1000")]
    //      float gravDelay = 50; // Used to determine how long it takes for the player ship to start being pulled in after thrust has stopped.
    //      private bool activeGravity = false; // Indicates whether the player ship is in an active gravity zone or not.
    //      private bool isFacing = false;
    //
    //      public override void _Process(double delta)
    //      {
    //          // FacingCheck();
    //          // star.UpdateGravity(this);
    //      }

    //      if (Input.IsActionPressed("thrust")){
    //         // AS LONG AS THE PLAYER IS HOLDING "THRUST" THEN STEP VELOCITY UPWARD UNTIL HITTING THRUSTSTRENGTH AND THEN MAINTAIN IT.
    //         // ACTIVE GRAVITY IMPLEMENTATION UNDER EVALUATION, MAY BE MOVED ELSEWHERE.
    //             if (activeGravity){
    //                 if (GlobalPosition.DistanceTo(star.GlobalPosition) < 50){
    //                     Velocity = Vector2.Zero;
    //                 }
    //                 // While under the influence of altered gravity the player's thrust speed is reduced
    //                 // else if (isFacing){
    //                 //     Velocity = Velocity.MoveToward(Transform.X * (thrustStrength + -gravityStrength), thrustStrength / drag);
    //                 // }
    //                 else {
    //                     Velocity = Velocity.MoveToward(Transform.X * (thrustStrength + star.GravStrength), thrustStrength / drag);
    //                 }
    //             }
    //             else{
    //                 Velocity = Velocity.MoveToward(Transform.X * thrustStrength, thrustStrength / drag);
    //             }
    //         }
    //         else{
    //             if (activeGravity){
    //                 // Pull the player towards the star, simulating gravity.
    //                 Velocity = Velocity.MoveToward(star.GlobalPosition * (star.GravDir * star.GravStrength * (float)delta), star.GravRadius / gravDelay);
    //                 // ***** OLD IMPLEMENTATION, MAY BE OF USE FOR SETTING UP SOLAR SYSTEM ***** \\
    //                 // Velocity += -(star.GravDir * star.MaxGrav * (float)delta);
    //                 if (GlobalPosition.DistanceTo(star.GlobalPosition) < 50){
    //                     Velocity = Vector2.Zero;
    //                 }
    //             }
    //             else{
    //                 // Once "thrust" input stops then step velocity downward until hitting 0.
    //                 Velocity = Velocity.MoveToward(Transform.X * 0, thrustStrength / decayStrength);
    //             }
    //
    // DETERMINE IF THE PLAYER IS FACING THE "STAR" OBJECT. COULD BE ADJUSTED TO BE USED FOR ANY OBJECT.
    // IMPLEMENTATION UNDER EVALUATION, MAY BE MIGRATED ELSEWHERE.
    // private void FacingCheck(){
    //     Vector2 playerFacing = (star.GlobalPosition - GlobalPosition).Normalized(); // Contains a unit vector (normal) that points to the "star" object.
    //     //*** UNCOMMENT FOR DEBUG HELP ***\\
    //     // GD.Print(playerFacing.Dot(Transform.X));

    //     // When used on a unit vector (playerFacing) the product of the dot method, which is a scalar, will always return between -1 (180 deg) and 1 (0 deg).
    //     // This can be leveraged to determine if the player is facing the "star" or not by comparing the dot product to the playerFov value, as below.
    //     if (playerFacing.Dot(Transform.X) > playerFov){
    //         isFacing = true;
    //     }
    //     else{
    //         isFacing = false;
    //     }
    //     // GD.Print($"Ship is facing star: {isFacing}");
    // }
    //     // ACTIVE GRAVITY IMPLEMENTATION UNDER EVALUATION, MAY BE MIGRATED ELSEWHERE.
    //     private void OnGravityAltered(Node2D body, bool status){
    //     if (body is Player){
    //         activeGravity = status;
    //     }
    // }
