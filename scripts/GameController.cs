
using System.Collections.Generic;
using System.Linq;

public partial class GameController : Node
{
    private Dealer dealer;
    private Container victory;
    private List<Space> spaces;
    private List<Space> foundations;

    private Space stock;
    private bool alwaysStack;

    private Card lastWaste;
    private readonly DragState dragState = new ();

    const string configPath = "user://state.sav";

    public override void _Ready()
    {
        dealer = GetNode<Dealer>("%Dealer");
        victory = GetNode<Container>("%Victory");
        spaces = [.. GetChildren().OfType<Space>()];
        foundations = [.. spaces.Where(s => s.Location == LocationType.Foundation)];

        stock = spaces.Single(s => s.Location == LocationType.Stock);

        NewGame(false);

        ConfigureUI();

        var config = new ConfigFile();
        if(config.Load(configPath) == Error.Ok)
            GetNode<Label>("%WinsValue").Text = config.GetValue("stats", "wins", "0").ToString();
    }

    private void NewGame(bool sameSeed)
    {
        lastWaste = null;
        victory.Visible = false;
        spaces.ForEach(s => s.Child = null);
        stock.Location = LocationType.Stock;
        stock.SetFace();
        dealer.ShuffleNewDeck(sameSeed);

        // deal tableaus
        for (var i = 1; i <= 7; i++) {
            var tableau = GetNode<Space>("%Tableau" + i);
            Card last = null;
            for(var j = 0; j < i; j++) {
                var next = dealer.DealCard((ICanParent)last ?? tableau, stock.GlobalPosition);
                if(next != null)
                    last = next;
            }
        }

        dealer.AnimateMove(true);
    }

    private void ConfigureUI()
    {
        GetNode<Button>("%NewGame").Pressed += () => {
            if(dealer.Dealing)
                return;
            NewGame(GetNode<CheckBox>("%SameSeed").ButtonPressed);
        };

        GetNode<Button>("%Hint").Pressed += () => {
            if (victory.Visible)
                return;
            var allCards = dealer.AllCards();
            foreach(var c in allCards.Where(c => c.CanBeDragged()))
            {
                if(allCards.Exists(o => o.Child != c && o.CanAccept(c)))
                    c.Flash();
                else if(spaces.Exists(o => o.CanAccept(c)))
                    c.Flash();
            }
        };

        GetNode<Button>("%Stack").Pressed += () => {
            if(dealer.Dealing || victory.Visible)
                return;
            TryStackOnFoundations();
        };

        GetNode<CheckBox>("%AlwaysStack").Pressed += () => {
            if(dealer.Dealing || victory.Visible)
                return;
            alwaysStack = GetNode<CheckBox>("%AlwaysStack").ButtonPressed;
            if(alwaysStack)
                GetNode<Button>("%Stack").Disabled = alwaysStack;
        };
    }

    public override void _Input(InputEvent @event)
    {
        if(dealer.Dealing || victory.Visible)
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
                    if (p is Space s && s.Location == LocationType.Foundation)
                        CheckForWin();
                }
                else // return to start
                {
                    dealer.QueueMove(dragState.Card, true);
                    dragState.Reset();
                    dealer.AnimateMove(false);
                }
            } 
            else if (!dragState.Active && mb.Pressed)
            {
                var under = UnderPoint(mb.GlobalPosition);
                if (under is Card c && c.CanBeDragged())
                {
                    dragState.Init(c, mb.GlobalPosition);
                }
                else if (under is Space s && s.Location == LocationType.Stock)
                {
                    for(var i = 0; i < 3; i++)
                    {
                        var next = dealer.DealCard(lastWaste, stock.GlobalPosition);
                        if(next != null && lastWaste == null)
                        {
                            next.GlobalPosition = GetNode<Area2D>("%Waste").GlobalPosition;
                            next.Location = LocationType.Waste;
                        }
                        lastWaste = next;
                    }
                    dealer.AnimateMove(true);
                    if(dealer.DeckEmpty)
                    {
                        s.Location = LocationType.Spare;
                        s.SetFace();
                    }
                }
            }
            if(alwaysStack)
                TryStackOnFoundations();
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
                var parent = area.GetParent();
                if(parent is Card c && dragState.Card != c && (top == null || top is not Card _ || top.ZIndex < c.ZIndex))
                    top = c;
                else if(parent is Space s && top == null)
                    top = s;
            }
        }

        return top;
    }

    private void TryStackOnFoundations()
    {
        var nextRank = foundations.Select(f => f.TopCard()?.Rank + 1 ?? 1).Min();
        var allCards = dealer.AllCards();
        foreach(var f in foundations)
        {
            var top = f.TopCard();
            if(top != null && top.Rank > nextRank)
                continue;

            var next = allCards.Where(c => c.Rank == nextRank && (top == null || c.Suit == top.Suit) && c.Child == null && c.CanBeDragged()).FirstOrDefault();
            if(next == null)
                continue;
            
            if(next == lastWaste)
                lastWaste = lastWaste.Parent as Card;

            next.ChangeParent(top);
            top.PositionChild(next);

            TryStackOnFoundations();
            return;
        }
    }

    private void CheckForWin()
    {
        if(foundations.Any(f => f.TopCard()?.Rank != 13))
            return;
        
        victory.Show();

        var wins = 0;        
        var config = new ConfigFile();
        if(config.Load(configPath) == Error.Ok)
            if(int.TryParse(config.GetValue("stats", "wins", "0").ToString(), out var saved))
                wins = saved;
        wins++;
        config.SetValue("stats", "wins", wins.ToString());
        GetNode<Label>("%WinsValue").Text = wins.ToString();
    }
}
