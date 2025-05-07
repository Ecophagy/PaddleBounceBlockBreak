using Microsoft.Xna.Framework;

namespace PaddleBounceBlockBreak.Components;

public class PositionComponent
{
    public Vector2 Position;

    public PositionComponent(Vector2 startingPosition)
    {
        Position = startingPosition;
    }
}