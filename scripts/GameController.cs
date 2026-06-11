

public partial class GameController : Node
{
    public override void _Ready()
    {
        GD.Print("hello from codium!");

        // foreach(var o in GetChildren())
        //     if (o is Card c)
        //         c.SetFace(c.Suit, c.Rank);
    }

    Card draggedCard;
    Vector2 dragStart, dragOffset;

    const int dragZIndex = 1000;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            if(draggedCard != null && !mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanAccept(draggedCard))
                {
                    c.Child = draggedCard;
                    c.PositionOnTop(draggedCard);
                    // disconnect old parent?
                    draggedCard = null;
                }
                else if (under is Space s && s.CanAccept(draggedCard))
                {
                    s.Child = draggedCard;
                    s.PositionOnSpace(draggedCard);
                    // disconnect old parent?
                    draggedCard = null;
                }
                else
                {
                    draggedCard.GlobalPosition = dragStart;
                    if(draggedCard.Child != null)
                        draggedCard.PositionOnTop(draggedCard.Child);
                    // reset z order
                    draggedCard = null;
                }
            } 
            else if (draggedCard == null && mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanBeDragged())
                {
                    c.ZIndex = dragZIndex;
                    dragStart = c.GlobalPosition;
                    dragOffset = c.GlobalPosition - mb.GlobalPosition;
                    draggedCard = c;
                }
                else if (under is Stock s)
                {
                    s.ToWaste();
                }
            }
        }
        else if(draggedCard != null && @event is InputEventMouseMotion mm)
        {
            draggedCard.GlobalPosition = mm.GlobalPosition + dragOffset;
            if(draggedCard.Child != null)
                draggedCard.PositionOnTop(draggedCard.Child);
        }
    }

    private GodotObject UnderPoint(Vector2 point)
    {
        var query = new PhysicsPointQueryParameters2D
        {
            Position = point,
            CollisionMask = 1,
            CollideWithAreas = true,
            CollideWithBodies = false
        };
        var results = GetViewport().World2D.DirectSpaceState.IntersectPoint(query);
        Sprite2D top = null;
        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Area2D area) 
            {
                var parent = area.GetParent();
                if(parent is Card c && (draggedCard == null || draggedCard != c) && (top == null || top.ZIndex < c.ZIndex))
                    top = c;
                else if(parent is Space s && top == null)
                    top = s;
            }
        }
        return top;
    }
}
