
public partial class Space : Sprite2D, ICanParent
{
    public Card Child { get; set; }
    [Export]
    public LocationType Location { get; set; }

    public bool CanAccept(Card other)
    {
        if (Child != null)
            return false;
        if (Location == LocationType.Foundation)
            return other.Rank == 1 && other.Child == null;
        if(Location == LocationType.Tableau)
            return true;
        return other.Child == null;
    }
    
    public void PositionChild(Card other)
    {
        other.GlobalPosition = GlobalPosition;
        other.ZIndex = ZIndex + 1;
        other.Location = Location;
        if(other.Child != null)
            other.PositionChild(other.Child);
    }
}