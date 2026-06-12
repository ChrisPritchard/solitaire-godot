
public interface ICanParent
{
    Card Child { get; set; }
    LocationType Location { get; set; }

    bool CanAccept(Card child);
    void PositionChild(Card child);
}