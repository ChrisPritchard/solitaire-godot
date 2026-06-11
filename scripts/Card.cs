
public partial class Card : Sprite2D
{
    public Card Child { get; set; }
    [Export]
    public LocationType Location { get; set; }

    [Export]
    public int Suit { get; set; }

    [Export]
    public int Rank { get; set; }

    public readonly Vector2 TableauOffset = new(0, 10);

    public void SetFace(int suit, int rank)
    {
        Suit = suit;
        Rank = rank;
        RegionEnabled = true;
        RegionRect = SpriteRegions.CardIndexes[Suit][Rank];
    }

    public bool CanAccept(Card other)
    {
        if(Child != null) 
            return false;

        if(Location == LocationType.Foundation)
        {
            if (other.Child != null)
                return false;

            return other.Suit == Suit && other.Rank == Rank + 1;
        }
        else if (Location == LocationType.Tableau)
        {
            return Suit%2 != other.Suit%2 && Rank == other.Rank+1;
        }

        return false;
    }

    public void PositionOnTop(Card other)
    {
        if(Location == LocationType.Foundation)
            other.GlobalPosition = GlobalPosition;
        else
            other.GlobalPosition = GlobalPosition + TableauOffset;

        other.ZIndex = ZIndex + 1;
        other.Location = Location;

        if(other.Child != null)
            other.PositionOnTop(other.Child);
    }

    public bool CanBeDragged()
    {
        if (Location == LocationType.Foundation)
            return false;

        if(Location == LocationType.Waste && Child != null)
            return false;

        if (Location == LocationType.Tableau && (Child == null || Child.CanBeDragged()))
            return true;

        return false;
    }
}
