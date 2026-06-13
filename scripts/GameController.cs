

using System.Collections.Generic;

public partial class GameController : Node
{
    readonly Queue<(int Suit, int Rank)> deck = [];

    private PackedScene cardScene;

    public override void _Ready()
    {
        // create random deck
        var allCards = new List<(int Suit, int Rank)>();
        for (var i = 0; i < 4; i++)
            for(var j = 1; j < 14; j++)
                allCards.Add((i, j));

        var random = new Random(1);
        while (deck.Count < 52)
        {
            var index = random.Next(allCards.Count);
            deck.Enqueue(allCards[index]);
            allCards.RemoveAt(index);
        }

        cardScene = ResourceLoader.Load<PackedScene>("res://scenes/card.tscn");

        // deal tableaus
        for (var i = 1; i <= 7; i++) {
            var tableau = GetNode<Space>("%Tableau" + i);
            Card last = null;
            for(var j = 0; j < i; j++) {
                var next = DealCard((ICanParent)last ?? tableau);
                if(next != null)
                    last = next;
            }
        }
    }

    private Card DealCard(ICanParent parent)
    {
        if(deck.Count == 0)
            return null;

        var (Suit, Rank) = deck.Dequeue();
        var card = cardScene.Instantiate<Card>();
        card.SetFace(Suit, Rank);
        
        if(parent != null)
        {
            card.ChangeParent(parent);
            parent.PositionChild(card);
        }

        AddChild(card);
        return card;
    }

    Card lastWaste;

    Card draggedCard;
    Vector2 dragStart, dragOffset;
    int startZIndex;

    const int dragZIndex = 1000;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            if(draggedCard != null && !mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is ICanParent p && p.CanAccept(draggedCard))
                {
                    if(draggedCard == lastWaste)
                        lastWaste = lastWaste.Parent as Card;
                    draggedCard.ChangeParent(p);
                    p.PositionChild(draggedCard);
                    draggedCard = null;
                }
                else
                {
                    draggedCard.GlobalPosition = dragStart;
                    if(draggedCard.Child != null)
                        draggedCard.PositionChild(draggedCard.Child);
                    draggedCard.ZIndex = startZIndex;
                    draggedCard = null;
                }
            } 
            else if (draggedCard == null && mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanBeDragged())
                {
                    startZIndex = c.ZIndex;
                    dragStart = c.GlobalPosition;
                    dragOffset = c.GlobalPosition - mb.GlobalPosition;
                    draggedCard = c;
                }
                else if (under is Stock s)
                {
                    for(var i = 0; i < 3; i++)
                    {
                        var next = DealCard(lastWaste);
                        if(next != null && lastWaste == null)
                        {
                            next.GlobalPosition = GetNode<Area2D>("%Waste").GlobalPosition;
                            next.Location = LocationType.Waste;
                        }
                        lastWaste = next;
                    }
                    if(deck.Count <= 3)
                        s.QueueFree();
                }
            }
        }
        else if(draggedCard != null && @event is InputEventMouseMotion mm)
        {
            draggedCard.ZIndex = dragZIndex;
            draggedCard.GlobalPosition = mm.GlobalPosition + dragOffset;
            if(draggedCard.Child != null)
                draggedCard.PositionChild(draggedCard.Child);
        }
    }

    private CanvasItem UnderPoint(Vector2 point)
    {
        var query = new PhysicsPointQueryParameters2D
        {
            Position = point,
            CollisionMask = 1,
            CollideWithAreas = true,
            CollideWithBodies = false
        };

        var results = GetViewport().World2D.DirectSpaceState.IntersectPoint(query);
        CanvasItem top = null;

        foreach (var result in results)
        {
            if (result["collider"].AsGodotObject() is Area2D area) 
            {
                var parent = area.GetParent<GodotObject>();
                if(parent is Card c && (draggedCard == null || draggedCard != c) && (top == null || top.ZIndex < c.ZIndex))
                    top = c;
                else if(parent is Space s && top == null)
                    top = s;
                else if(parent is Stock o)
                    top = o;
            }
        }
        return top;
    }
}
