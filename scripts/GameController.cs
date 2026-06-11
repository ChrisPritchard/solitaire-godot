
using System.Collections.Generic;

public partial class GameController : Node
{
    public override void _Ready()
    {
        GD.Print("hello from codium!");
    }

    readonly List<(Card Card, Vector2 DragStart)> dragged = [];

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            if(dragged.Count > 0 && !mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanAccept(dragged[0].Card))
                {
                    c.Child = dragged[0].Card;
                    // set position
                    dragged.Clear();
                }
                else if (under is Space s && dragged.Count == 1 && s.CanAccept(dragged[0].Card))
                {
                    s.Child = dragged[0].Card;
                    // set position
                    dragged.Clear();
                }
                // reset positions
                dragged.Clear();
            } 
            else if (dragged.Count == 0 && mb.Pressed)
            {
                // start dragging
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanBeDragged())
                {
                    // set dragged to card
                }
                else if (under is Stock s)
                {
                    s.ToWaste();
                }
            }
        }
        else if(dragged.Count > 0 && @event is InputEventMouseMotion mm)
        {
            foreach(var (card, dragStart) in dragged)
            {
                card.GlobalPosition = mm.GlobalPosition + dragStart;
            }
        }
    }

    private GodotObject UnderPoint(Vector2 point)
    {
        throw new NotImplementedException();
    }
}
