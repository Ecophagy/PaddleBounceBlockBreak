using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaddleBounceBlockBreak.Sprites
{
    public class Ball : Sprite
    {
        private float _timer = 0f; // Increase speed over time
        private Vector2? _startPosition = null;
        private float? _startSpeed;
        private bool _isPlaying;

        public Score Score; // TODO: Not used here?
        public int SpeedIncrementInterval = 10; // How often speed will increase

        public Ball(Texture2D texture) : base(texture)
        {
            Speed = 3f;
        }

        public void Update(GameTime gameTime, List<Block> blocks, Sprite paddle)
        {
            // Set initial values
            if (_startPosition == null)
            {
                _startPosition = Position;
                _startSpeed = Speed;

                Restart();
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _isPlaying = true;
            }

            if(!_isPlaying)
            {
                return;
            }

            else
            {
                // Update speed if SpeedIncrementInterval has elapsed
                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer > SpeedIncrementInterval)
                {
                    Speed++;
                    _timer = 0;
                }

                _ = HandleSpriteCollision(paddle);

                foreach (var block in blocks)
                {
                    if (HandleSpriteCollision(block))
                    {
                        // We hit a block! Tell it about it
                        block.BlockHit();
                    }
                }

                // Update Velocity from collision with walls
                if (Position.X <= 0 || Position.X + _texture.Width >= Game1.ScreenWidth)
                {
                    Velocity.X = -Velocity.X;
                }
                if (Position.Y <= 0)
                {
                    Velocity.Y = -Velocity.Y;

                }
                if (Position.Y + _texture.Height >= Game1.ScreenHeight)
                {
                    // GAME OVER
                    Restart();
                }

                Position += Velocity * Speed;
            }
        }

        // Return if a collision occurred with parameter sprite
        private bool HandleSpriteCollision(Sprite sprite)
        {
            if (sprite == this)
            {
                return false;
            }

            // Update Velocity on collisions with other sprite
            if (this.Velocity.X > 0 && this.IsTouchingLeft(sprite))
            {
                this.Velocity.X = -this.Velocity.X;
                return true;
            }
            if (this.Velocity.X < 0 && this.IsTouchingRight(sprite))
            {
                this.Velocity.X = -this.Velocity.X;
                return true;
            }
            if (this.Velocity.Y > 0 && this.IsTouchingTop(sprite))
            {
                this.Velocity.Y = -this.Velocity.Y;
                return true;
            }
            if (this.Velocity.Y < 0 && this.IsTouchingBottom(sprite))
            {
                this.Velocity.Y = -this.Velocity.Y;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Restart()
        {
            // Randomly set starting velocity direction
            var direction = Game1.Random.Next(0, 4);
            switch (direction)
            {
                case 0:
                    Velocity = new Vector2(1, 1);
                    break;
                case 1:
                    Velocity = new Vector2(1, -1);
                    break;
                case 2:
                    Velocity = new Vector2(-1, -1);
                    break;
                case 3:
                    Velocity = new Vector2(-1, 1);
                    break;
            }

            Position = (Vector2)_startPosition;
            Speed = (float)_startSpeed;
            _timer = 0;
            _isPlaying = false;
        }
        
    }
}
