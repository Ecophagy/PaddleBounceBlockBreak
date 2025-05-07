using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class MotionSystem
{
    public void Update(MotionComponent motion, PositionComponent position)
    {
        position.Position += motion.Velocity; // TODO: Take time into account? 
        // TODO: clamp position
        //Position.X = MathHelper.Clamp(Position.X, 0, Game1.ScreenWidth - _texture.Width); // Requires static screenwidth
    }
}