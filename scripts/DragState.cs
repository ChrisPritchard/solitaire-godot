
public class DragState
{
    public bool Active => Card != null;
    
    public Card Card { get; private set; }
    Vector2 dragStart, dragOffset;
    int startZIndex;

    const int dragZIndex = 1000;

    public void Init(Card card, Vector2 dragPoint)
    {
        startZIndex = card.ZIndex;
        dragStart = card.GlobalPosition;
        dragOffset = card.GlobalPosition - dragPoint;
        Card = card;
    }

    public void Update(Vector2 dragPoint)
    {
        Card.ZIndex = dragZIndex;
        Card.GlobalPosition = dragPoint + dragOffset;
        if(Card.Child != null)
            Card.PositionChild(Card.Child);
    }

    public void Reset()
    {
        Card.GlobalPosition = dragStart;
        Card.ZIndex = startZIndex;
        if(Card.Child != null)
            Card.PositionChild(Card.Child);
        Card = null;
    }

    public void Conclude(ICanParent parent)
    {
        Card.ChangeParent(parent);
        parent.PositionChild(Card);
        Card = null;
    }
}