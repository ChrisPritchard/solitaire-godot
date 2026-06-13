

using System.Collections.Generic;

public partial class GameController : Node
{
    readonly Queue<(int Suit, int Rank)> deck = [];

    private PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://scenes/card.tscn");

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
        Sfx.SFX.Deal();
        return card;
    }

    Card lastWaste;
    readonly DragState dragState = new ();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            if(dragState.Active && !mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is ICanParent p && p.CanAccept(dragState.Card))
                {
                    if(dragState.Card == lastWaste)
                        lastWaste = lastWaste.Parent as Card;
                    dragState.Conclude(p);
                }
                else // return to start
                    dragState.Reset();
            } 
            else if (!dragState.Active && mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanBeDragged())
                    dragState.Init(c, mb.GlobalPosition);
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
        else if(dragState.Active && @event is InputEventMouseMotion mm)
            dragState.Update(mm.GlobalPosition);
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
                if(parent is Card c && dragState.Card != c && (top == null || top.ZIndex < c.ZIndex))
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
