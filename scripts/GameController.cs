

public partial class GameController : Node
{
    public override void _Ready()
    {
        GD.Print("hello from codium!");
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
                    draggedCard = null;
                }
                else if (under is Space s && s.CanAccept(draggedCard))
                {
                    s.Child = draggedCard;
                    s.PositionOnSpace(draggedCard);
                    draggedCard = null;
                }
                else
                {
                    draggedCard.GlobalPosition = dragStart;
                    if(draggedCard.Child != null)
                        draggedCard.PositionOnTop(draggedCard.Child);
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
        throw new NotImplementedException();
    }
}
