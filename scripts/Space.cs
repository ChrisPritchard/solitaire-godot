
public partial class Space : Sprite2D, ICanParent
{
    public Card Child { get; set; }
    [Export]
    public LocationType Location { get; set; }

    public override void _Ready()
    {
        SetFace();
    }

    public void SetFace()
    {   
        Texture = ResourceLoader.Load<Texture2D>("res://assets/card-suites.png");
        RegionEnabled = true;

        if(Location == LocationType.Stock)
            RegionRect = SpriteRegions.CardBack;
        else
            RegionRect = SpriteRegions.CardSpace;
    }

    public bool CanAccept(Card other)
    {
        if (Child != null)
            return false;
        if (Location == LocationType.Foundation)
            return other.Rank == 1 && other.Child == null;
        if(Location == LocationType.Tableau)
            return true;
        if(Location == LocationType.Stock)
            return false;
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

    public Card TopCard() => Child?.TopCard();
}