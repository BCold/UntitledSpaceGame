using Godot;

public partial class OriginalPlayer : CharacterBody2D
{
    // Export Variables
    [ExportGroup("Movement Properties")]
    [Export(hint: PropertyHint.Range, hintString: "-90, 90")]
    float rotationStrength = 0;
    [Export(hint: PropertyHint.Range, hintString: "-1000, 1000")]
    float thrustStrength = 0;

    private float _rotationDirection;

    private float thrustDecay = 100;
    private bool isDecelerating = false;
    private bool isAccelerating = false;

    private Timer decayTimer; // Used to calculate deceleration values
    private Timer accelTimer; // Used to calculate acceleration values

    public override void _Ready()
    {
        decayTimer = GetNode<Timer>("DecelTimer");
        accelTimer = GetNode<Timer>("AccelTimer");
    }


    public void GetInput(){
        // Set rotation direction based on player input
        _rotationDirection = Input.GetAxis("rotate_left", "rotate_right");
        // Set velocity strength when thrust input is active
        if (Input.IsActionJustPressed("thrust")){
            isDecelerating = false;
            decayTimer.Stop();

            isAccelerating = true;
            accelTimer.Start();

        }
        if (Input.IsActionPressed("thrust")){
            if (isAccelerating){
                Accelerate();
            }
            else{
                Velocity = Transform.X * thrustStrength;
            }
        }
        else if(Input.IsActionJustReleased("thrust")){
            isDecelerating = true;
            decayTimer.Start();
        }
    }

// ---------- THE BELOW IS A WORKING, BUT FRAGILE APPROACH TO ACCELERATION AND DECELERATION. WORKSHOP AND TEST ALTERNATIVE SOLUTIONS ---------- //
    private void Decelerate(){
    // Calculate the deceleration using the time left on the decay timer
        Velocity = Transform.X * ((float)decayTimer.TimeLeft * thrustStrength);
        // GD.Print((float)decayTimer.TimeLeft * thrustStrength);
    }
    private void Accelerate(){
    // Calculate the acceleration using the time left on the accel timer
        Velocity = Transform.X * (thrustStrength * (1 - (float)accelTimer.TimeLeft));
        GD.Print(1- (float)accelTimer.TimeLeft);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        if (isDecelerating){
           Decelerate();
        }
        Rotate(_rotationDirection * rotationStrength * (float)delta);
        MoveAndSlide();
    }

    private void OnDecayTimeout(){
        isDecelerating = false;
        decayTimer.Stop();
        Velocity = Vector2.Zero;
    }

    private void OnAccelTimeout(){
        isAccelerating = false;
        accelTimer.Stop();
    }
}
