using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PaddleBounceBlockBreak.Sprites
{
    public class Paddle : Sprite
    {
        public Paddle(Texture2D texture) : base(texture)
        {
            Speed = 5f;
        }

        public override void Update(GameTime gameTime)
        {
            if(Input == null)
            {
                throw new Exception("Input has no value");
            }

            if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Velocity.X = -Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Velocity.X = Speed;
            }

            Position += Velocity;

            Position.X = MathHelper.Clamp(Position.X, 0, Game1.ScreenWidth - _texture.Width); // Requires static screenwidth

            Velocity = Vector2.Zero;
        }
    }
}
