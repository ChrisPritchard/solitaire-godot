
using System.Security.Cryptography.X509Certificates;

public partial class Card : Sprite2D, ICanParent
{
    public ICanParent Parent { get; set; }
    public Card Child { get; set; }
    [Export]
    public LocationType Location { get; set; }

    [Export]
    public int Suit { get; set; }

    [Export]
    public int Rank { get; set; }

    public readonly Vector2 TableauOffset = new(0, 25);
    public readonly Vector2 WasteOffset = new(25, 0);

    public void SetFace(int suit, int rank)
    {
        Suit = suit;
        Rank = rank;

        Texture = ResourceLoader.Load<Texture2D>("res://assets/card-suites.png");
        RegionEnabled = true;
        RegionRect = SpriteRegions.CardIndexes[Suit][Rank];
    }

    public bool CanAccept(Card other)
    {
        if(other != Child && Child != null) 
            return false;

        if(Location == LocationType.Foundation)
        {
            if (other.Child != null)
                return false;

            return other.Suit == Suit && other.Rank == Rank + 1;
        }
        else if (Location == LocationType.Tableau)
            return Suit%2 != other.Suit%2 && Rank == other.Rank+1;

        return false;
    }

    public void ChangeParent(ICanParent other)
    {
        if(Parent != null)
            Parent.Child = null;
        Parent = other;
        other.Child = this;
    }

    public void PositionChild(Card other)
    {
        if(Location == LocationType.Foundation)
            other.GlobalPosition = GlobalPosition;
        else if(Location == LocationType.Waste)
            other.GlobalPosition = GlobalPosition + WasteOffset;
        else
            other.GlobalPosition = GlobalPosition + TableauOffset;

        other.ZIndex = ZIndex + 1;
        other.Location = Location;

        if(other.Child != null)
            other.PositionChild(other.Child);
    }

    public Card TopCard() => Child?.TopCard() ?? this;

    public bool CanBeDragged()
    {
        if (Location == LocationType.Foundation)
            return false;

        if(Location == LocationType.Waste)
            return Child == null;

        if (Location == LocationType.Tableau)
            return Child == null || (CanAccept(Child) && Child.CanBeDragged());

        return false;
    }

    public void Flash()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", Colors.Magenta, 0.2f);
        tween.TweenProperty(this, "modulate", Colors.White, 0.2f);
    }
}
