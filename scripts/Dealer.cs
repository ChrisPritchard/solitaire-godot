using System.Collections.Generic;

public partial class Dealer : Node
{
    public bool Dealing => dealing > 0;

    [Export]
    public Node2D CardSpawn { get; set; }

    private int dealing;

    private readonly Queue<Card> queue = [];
    private readonly Queue<Vector2> returnStarts = [];

    const int dealerZIndex = 1000;

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