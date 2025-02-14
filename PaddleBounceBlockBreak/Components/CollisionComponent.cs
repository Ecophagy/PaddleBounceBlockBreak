namespace PaddleBounceBlockBreak.Components;

public class CollisionComponent
{
    public int Height {get;}
    public int Width {get;}

    public bool Impact { get; set; } = false;

    public CollisionComponent(int height, int width)
    {
        Height = height;
        Width = width;
    }
}