using Microsoft.Xna.Framework;
using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class AccelerationSystem
{
    public void Update(GameTime gameTime, AccelerationComponent acceleration, MotionComponent motion)
    {
        acceleration.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (acceleration.Timer > acceleration.SpeedIncrementInterval)
        {
            motion.Velocity *= acceleration.Acceleration;
            acceleration.Timer = 0;
        }
    }
}