
public partial class Space : Node2D
{
    public Card Child { get; set; }
    public LocationType Location { get; set; }

    public bool CanAccept(Card other) => Child != null && other.Child == null;
}