using System;
using Godot;

public partial class BaseCard : Node2D
{
    private Area2D area;

    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += HandleInputEvent;
    }

    private void HandleInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
        {
            GD.Print("clicked card");
        }
    }
}
