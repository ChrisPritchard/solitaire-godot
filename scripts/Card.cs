
public partial class Card : Node2D
{
    public GodotObject Parent { get; set; }
    public Card Child { get; set; }
    public LocationType Location { get; set; }

    public (int Suit, int Rank) Face { get; set; }

    public readonly Vector2 TableauOffset = new(0, 10);

    public bool CanAccept(Card other)
    {
        if(Child != null) 
            return false;

        if(Location == LocationType.Foundation)
        {
            if (other.Child != null)
                return false;
                
            return other.Face.Suit == Face.Suit && other.Face.Rank == Face.Rank + 1;
        }
        else if (Location == LocationType.Tableau)
        {
            return Face.Suit%2 != other.Face.Suit%2 && Face.Rank == other.Face.Rank+1;
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
