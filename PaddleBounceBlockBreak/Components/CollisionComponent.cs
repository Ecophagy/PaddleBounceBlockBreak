namespace PaddleBounceBlockBreak.Components;

public class CollisionComponent
{
    public int Height {get;}
    public int Width {get;}

    public CollisionComponent(int height, int width)
    {
        Height = height;
        Width = width;
    }
}