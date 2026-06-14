
public partial class GameController : Node
{
    private Dealer dealer;

    public override void _Ready()
    {
        dealer = GetNode<Dealer>("%Dealer");
        NewGame(false);

        GetNode<Button>("%NewGame").Pressed += () => {
            if(dealer.Dealing)
                return;
            NewGame(GetNode<CheckBox>("%SameSeed").ButtonPressed);
        };
    }

    private void NewGame(bool sameSeed)
    {
        lastWaste = null;
        dealer.InitDeck(sameSeed);

        // deal tableaus
        for (var i = 1; i <= 7; i++) {
            var tableau = GetNode<Space>("%Tableau" + i);
            Card last = null;
            for(var j = 0; j < i; j++) {
                var next = dealer.DealCard((ICanParent)last ?? tableau);
                if(next != null)
                    last = next;
            }
        }

        dealer.AnimateDeal();
    }

    Card lastWaste;
    readonly DragState dragState = new ();

    public override void _Input(InputEvent @event)
    {
        if(dealer.Dealing)
            return;
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
                {
                    dealer.QueueReturn(dragState.Card);
                    dragState.Reset();
                    dealer.AnimateReturn();
                }
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
                        var next = dealer.DealCard(lastWaste);
                        if(next != null && lastWaste == null)
                        {
                            next.GlobalPosition = GetNode<Area2D>("%Waste").GlobalPosition;
                            next.Location = LocationType.Waste;
                        }
                        lastWaste = next;
                    }
                    dealer.AnimateDeal();
                    if(dealer.DeckEmpty)
                        s.Visible = false;
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
