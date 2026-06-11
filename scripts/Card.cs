
public partial class Card : Node2D
{
    public GodotObject Parent { get; set; }
    public Card Child { get; set; }
    public LocationType Location { get; set; }

    public bool CanAccept(Card other)
    {
        if(Child != null) return false;
        throw new NotImplementedException();
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
