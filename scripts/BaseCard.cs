
public partial class BaseCard : Node2D
{
    private Area2D area;

    private bool dragging;
    private Vector2 dragstart;

    public override void _Ready()
    {
        area = GetNode<Area2D>("Area2D");
        area.InputEvent += HandleInputEvent;
    }

    private void HandleInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
        {
            if (dragging && !mouse.Pressed)
                dragging = false;
            else if (!dragging && mouse.Pressed)
            {
                dragging = true;
                dragstart = GlobalPosition - GetGlobalMousePosition();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (dragging)
            GlobalPosition = GetGlobalMousePosition() + dragstart;
    }
}
