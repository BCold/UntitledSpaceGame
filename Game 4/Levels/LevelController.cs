using Godot;
using System;
using System.Linq;

public partial class LevelController : Node2D
{
    [Export]PackedScene PlayerScene;
    Vector2 windowSize;
    RectangleShape2D OnscreenArea;

    public override void _Ready()
    {
        windowSize = GetViewportRect().Size;
        OnscreenArea = (RectangleShape2D)GetNode<Area2D>("OnScreenArea").GetNode<CollisionShape2D>("CollisionShape2D").Shape;
        GD.Print($"Window size: {windowSize}");
        OnscreenArea.Size = windowSize;

        int index = 0;

        foreach(var item in GameManager.Players){
            Player currentPlayer = PlayerScene.Instantiate<Player>();
            currentPlayer.Name = item.Id.ToString();
            AddChild(currentPlayer);

            foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoint")){
                if (int.Parse(spawnPoint.Name) == index){
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                }
            }
            index++;
        }    
    }
    private void ObjectExitedScreen(Node2D body){
        // GD.Print($"{body.Name} exited screen, wrapping ...");
        // GD.Print($"{body.Name} global position is {body.GlobalPosition}");
        ScreenWrap(body);
    }

    private void ScreenWrap(Node2D body){
        // Screen left exited
        if (body.GlobalPosition.X < 0){
            body.GlobalPosition = new Vector2(x: -body.GlobalPosition.X + windowSize.X, y: body.GlobalPosition.Y);
            // Screen left and screen top exited
            if (body.GlobalPosition.Y < 0){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: -body.GlobalPosition.Y + windowSize.Y);
            }
            // Screen left and screen bottom exited
            else if(body.GlobalPosition.Y > windowSize.Y){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: body.GlobalPosition.Y - windowSize.Y);
            }
        }
        // Screen right exited
        else if(body.GlobalPosition.X > windowSize.X){
            body.GlobalPosition = new Vector2(x: body.GlobalPosition.X - windowSize.X, y: body.GlobalPosition.Y);
            // Screen right and screen top exited
            if (body.GlobalPosition.Y < 0){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: -body.GlobalPosition.Y + windowSize.Y);
            }
            // Screen right and screen bottom exited
            else if(body.GlobalPosition.Y > windowSize.Y){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: body.GlobalPosition.Y - windowSize.Y);
            }
        }
        // Screen top exited
        else if (body.GlobalPosition.Y < 0){
            body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: -body.GlobalPosition.Y + windowSize.Y);
            // Screen top and screen left exited
            if (body.GlobalPosition.X < 0){
                body.GlobalPosition = new Vector2(x: -body.GlobalPosition.X + windowSize.X, y: body.GlobalPosition.Y);
            }
            // Screen top and screen right exited
            else if (body.GlobalPosition.X > windowSize.X){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X - windowSize.X, y: body.GlobalPosition.Y);
            }
        }
        // Screen bottom exited
        else if (body.GlobalPosition.Y > windowSize.Y){
            body.GlobalPosition = new Vector2(x: body.GlobalPosition.X, y: body.GlobalPosition.Y - windowSize.Y);
            // Screen top and screen left exited
            if (body.GlobalPosition.X < 0){
                body.GlobalPosition = new Vector2(x: -body.GlobalPosition.X + windowSize.X, y: body.GlobalPosition.Y);
            }
            // Screen top and screen right exited
            else if (body.GlobalPosition.X > windowSize.X){
                body.GlobalPosition = new Vector2(x: body.GlobalPosition.X - windowSize.X, y: body.GlobalPosition.Y);
            }
        }
    }
}
