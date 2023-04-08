using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace PaddleBounceBlockBreak.Sprites
{
    public class Ball : Sprite
    {
        private float _timer = 0f; // Increase speed over time
        private Vector2? _startPosition = null;
        private float? _startSpeed;
        private bool _isPlaying;

        public int SpeedIncrementInterval = 20; // How often speed will increase in seconds

        public Ball(Texture2D texture) : base(texture)
        {
            Speed = 3f;
        }

        public override void Update(GameTime gameTime)
        {
            // Set initial values
            if (_startPosition == null)
            {
                _startPosition = Position;
                _startSpeed = Speed;

                Restart();
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Space)) // TODO: move this logic into Level
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
                    Speed += 0.5f;
                    _timer = 0;
                }

                Position += Velocity * Speed;
            }
        }

        
        /// <summary>
        /// Handle collision with collidable sprites
        /// </summary>
        /// <param name="sprite">sprite to check for collision with</param>
        /// <returns>True if there was a collision with sprite</returns>
        public bool HandleSpriteCollision(Sprite sprite)
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

        /// <summary>
        /// Handles Ball collision with screen edges
        /// </summary>
        /// <returns>If "game over" occurred</returns>
        public bool HandleWallCollision(int ScreenWidth, int ScreenHeight)
        {
            var gameOver = false;
            // Update Velocity from collision with walls
            if (Position.X <= 0 || Position.X + _texture.Width >= ScreenWidth)
            {
                Velocity.X = -Velocity.X;
            }
            if (Position.Y <= 0)
            {
                Velocity.Y = -Velocity.Y;

            }
            if (Position.Y + _texture.Height >= ScreenHeight)
            {
                // GAME OVER
                gameOver = true;
            }
            return gameOver;
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
