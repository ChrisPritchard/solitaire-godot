using System.Collections.Generic;
using System.Linq;

public partial class Dealer : Node
{
    public bool Dealing => dealingCount > 0;

    public bool DeckEmpty => deck.Count == 0;

    private PackedScene cardScene = ResourceLoader.Load<PackedScene>("res://scenes/card.tscn");

    private int dealingCount;

    private readonly Queue<(int Suit, int Rank)> deck = [];
    private readonly Queue<(Card card, Vector2 start)> dealQueue = [];

    const int dealerZIndex = 1000;

    private int lastSeed;

    public void ShuffleNewDeck(bool sameSeed)
    {
        deck.Clear();
        foreach(var card in GetChildren().OfType<Card>())
            card.QueueFree();

        var allCards = new List<(int Suit, int Rank)>();
        for (var i = 0; i < 4; i++)
            for(var j = 1; j < 14; j++)
                allCards.Add((i, j)); 

        if(!sameSeed)
            lastSeed = new Random().Next();
        GD.Print("using seed ", lastSeed);
        var random = new Random(lastSeed);

        while (deck.Count < 52)
        {
            var index = random.Next(allCards.Count);
            deck.Enqueue(allCards[index]);
            allCards.RemoveAt(index);
        }
    }

    public List<Card> AllCards() => [.. GetChildren().OfType<Card>()];

    public Card DealCard(ICanParent parent, Vector2 cardSpawn)
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
        dealQueue.Enqueue((card, cardSpawn));
        return card;
    }

    public void QueueMove(Card card, bool withChild)
    {
        dealQueue.Enqueue((card, card.GlobalPosition));

        if(withChild && card.Child != null)
            QueueMove(card.Child, withChild);
    }

    public void AnimateMove(bool withGap)
    {
        if(Dealing)
            return;

        var delay = 0.0f;
        while(dealQueue.TryDequeue(out (Card card, Vector2 start) next))
        {
            dealingCount++;
            var tweener = CreateTween();
            var end = next.card.GlobalPosition;
            var endZ = next.card.ZIndex;
            next.card.Visible = false;
            
            if(withGap)
                tweener.TweenInterval(delay);
            tweener.TweenCallback(Callable.From(() => {
                next.card.GlobalPosition = next.start;
                next.card.ZIndex = dealerZIndex;
                next.card.Visible = true;
            }));
            tweener.TweenProperty(next.card, "global_position", end, 0.1f);
            tweener.Finished += () => 
            { 
                next.card.ZIndex = endZ; 
                Sfx.SFX.Deal(); 
                dealingCount--;
            };
            delay += 0.1f;
        }
    }
}