using Microsoft.Xna.Framework;

namespace PaddleBounceBlockBreak.Components;

public class AccelerationComponent
{
    public float Timer { get; set; } = 0f;
    public int SpeedIncrementInterval { get; } = 20;
    public Vector2 Acceleration { get; }

    public AccelerationComponent(Vector2 acceleration)
    {
        Acceleration = acceleration;
    }
}