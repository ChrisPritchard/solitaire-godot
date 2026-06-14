using System.Collections.Generic;
using System.Linq;

public partial class Dealer : Node
{
    public bool Dealing => dealing > 0;

    public bool DeckEmpty => deck.Count == 0;

    readonly Queue<(int Suit, int Rank)> deck = [];

    [Export]
    public Node2D CardSpawn { get; set; }

    private PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://scenes/card.tscn");

    private int dealing;

    private readonly Queue<Card> queue = [];
    private readonly Queue<Vector2> returnStarts = [];

    const int dealerZIndex = 1000;

    private int lastSeed;

    public void InitDeck(bool sameSeed)
    {
        deck.Clear();
        foreach(var card in GetChildren().OfType<Card>())
            card.QueueFree();

        // create random deck
        var allCards = new List<(int Suit, int Rank)>();
        for (var i = 0; i < 4; i++)
            for(var j = 1; j < 14; j++)
                allCards.Add((i, j));

        if(!sameSeed)
            lastSeed = new Random().Next();
        var random = new Random(lastSeed);

        while (deck.Count < 52)
        {
            var index = random.Next(allCards.Count);
            deck.Enqueue(allCards[index]);
            allCards.RemoveAt(index);
        }
    }

    public Card DealCard(ICanParent parent)
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
        QueueDeal(card);
        return card;
    }

    public void QueueDeal(Card card) => queue.Enqueue(card);

    public void QueueReturn(Card topCard)
    {
        queue.Enqueue(topCard);
        returnStarts.Enqueue(topCard.GlobalPosition);

        if(topCard.Child != null)
            QueueReturn(topCard.Child);
    }

    public void AnimateDeal()
    {
        if(Dealing)
            return;

        var delay = 0.0f;
        while(queue.TryDequeue(out Card next))
        {
            dealing++;
            var tweener = CreateTween();
            var end = next.GlobalPosition;
            var endZ = next.ZIndex;
            next.Visible = false;
            
            tweener.TweenInterval(delay);
            tweener.TweenCallback(Callable.From(() => {
                next.GlobalPosition = CardSpawn.GlobalPosition;
                next.ZIndex = dealerZIndex;
                next.Visible = true;
            }));
            tweener.TweenProperty(next, "global_position", end, 0.1f);
            tweener.Finished += () => 
            { 
                next.ZIndex = endZ; 
                Sfx.SFX.Deal(); 
                dealing--;
            };
            delay += 0.1f;
        }
    }

    public void AnimateReturn()
    {
        if(Dealing)
            return;

        while(queue.TryDequeue(out Card next))
        {
            dealing++;
            var start = returnStarts.Dequeue();
            var tweener = CreateTween();
            var end = next.GlobalPosition;
            var endZ = next.ZIndex;
            next.Visible = false;
            
            tweener.TweenCallback(Callable.From(() => {
                next.GlobalPosition = start;
                next.ZIndex = dealerZIndex;
                next.Visible = true;
            }));
            tweener.TweenProperty(next, "global_position", end, 0.1f);
            tweener.Finished += () => 
            { 
                next.ZIndex = endZ; 
                dealing--;
                if(dealing == 0)
                    Sfx.SFX.Deal(); 
            };
        }
    }
}