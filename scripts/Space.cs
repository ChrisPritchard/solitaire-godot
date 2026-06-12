
public partial class Space : Sprite2D, ICanParent
{
    public Card Child { get; set; }
    [Export]
    public LocationType Location { get; set; }

    public bool CanAccept(Card other)
    {
        if (Child != null || other.Child != null)
            return false;
        if (Location == LocationType.Foundation)
            return other.Rank == 1;
        return true;
    }
    
    public void PositionChild(Card other)
    {
        other.GlobalPosition = GlobalPosition;
        other.ZIndex = ZIndex + 1;
        other.Location = Location;
    }
}