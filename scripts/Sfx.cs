
public partial class Sfx : Node
{
    public static Sfx SFX { get; private set; }

    private AudioStreamPlayer drag, drop;

    public override void _Ready()
    {
        SFX = this;

        drag = GetNode<AudioStreamPlayer>("Drag");
        drop = GetNode<AudioStreamPlayer>("Drop");
    }

    public void Drag() => drag.Play();
    public void Drop() => drop.Play();
}