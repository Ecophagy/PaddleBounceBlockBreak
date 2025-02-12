using Microsoft.Xna.Framework;

namespace PaddleBounceBlockBreak.Components;

public class MotionComponent
{
    public Vector2 Velocity { get; set; } = new Vector2();
    public float Speed {get; set;}

    public MotionComponent(float speed)
    {
        Speed = speed;
    }
}