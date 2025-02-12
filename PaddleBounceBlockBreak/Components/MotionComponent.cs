using Microsoft.Xna.Framework;

namespace PaddleBounceBlockBreak.Components;

public class MotionComponent
{
    public Vector2 Velocity { get; set; }
    public float Speed {get; set;}

    public MotionComponent(float speed, Vector2 velocity)
    {
        Speed = speed;
        Velocity = velocity;
    }
}