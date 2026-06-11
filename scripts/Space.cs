
public partial class Space : Node2D
{
    public Card Child { get; set; }
    public LocationType Location { get; set; }

    public bool CanAccept(Card other)
    {
        if (Child != null || other.Child != null)
            return false;
        if (Location == LocationType.Foundation)
            return other.Face.Rank == 1;
        return true;
    }
    
    public void PositionOnSpace(Card other)
    {
        other.GlobalPosition = GlobalPosition;
        other.ZIndex = ZIndex + 1;
    }
}