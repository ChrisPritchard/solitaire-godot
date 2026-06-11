
public partial class Space : Node2D
{
    public Card Child { get; set; }
    public LocationType Location { get; set; }

    public bool CanAccept(Card other) => Child != null && other.Child == null;
    
    public void PositionOnSpace(Card other)
    {
        other.GlobalPosition = GlobalPosition;
        other.ZIndex = ZIndex + 1;
    }
}