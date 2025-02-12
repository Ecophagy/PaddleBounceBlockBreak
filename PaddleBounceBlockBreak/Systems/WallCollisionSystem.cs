using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class WallCollisionSystem
{
    public void Update(CollisionComponent collision, PositionComponent position, MotionComponent motion, int screenWidth, int screenHeight)
    {
        var gameOver = false;
        // Update Velocity from collision with walls
        if (position.Position.X <= 0 || position.Position.X + collision.Width >= screenWidth)
        {
            motion.Velocity = motion.Velocity with { X = -motion.Velocity.X };
        }
        if (position.Position.Y <= 0)
        {
            motion.Velocity = motion.Velocity with { Y = -motion.Velocity.Y };

        }
        if (position.Position.Y + collision.Height >= screenHeight)
        {
            // GAME OVER
            gameOver = true;
        }
        // FIXME: Do we return this? Or Emit an event?
        //return gameOver;
    }
}