using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class UserInputSystem
{
    public void Update(UserControlComponent userControl, MotionComponent motion)
    {
        if (Keyboard.GetState().IsKeyDown(userControl.Input.Left))
        {
            motion.Velocity = motion.Velocity with { X = -motion.Speed };
        }

        else if (Keyboard.GetState().IsKeyDown(userControl.Input.Right))
        {
            motion.Velocity = motion.Velocity with { X = motion.Speed };
        }

        else
        {
            motion.Velocity = motion.Velocity with { X = 0 };
        }
    }
}