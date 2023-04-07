using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Models;
using PaddleBounceBlockBreak.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaddleBounceBlockBreak
{
    public class Level : IDisposable
    {
        // Entities in the level
        private Ball _ball;
        private Paddle _paddle;
        private List<Block> _blocks;

        // Level State
        private Random _random = new Random();
        private bool _levelComplete = false;
        public int LevelScore = 0;

        // Level Content
        private ContentManager _content;

        public Level(IServiceProvider serviceProvider) // TODO: Can set difficulty/number of blocks here
        {
            _content = new ContentManager(serviceProvider, "Content");
            var paddleTexture = _content.Load<Texture2D>("paddle");
            var ballTexture = _content.Load<Texture2D>("ball");

            _paddle = new Paddle(paddleTexture)
            {
                Position = new Vector2((Game1.ScreenWidth / 2) - (paddleTexture.Width / 2), Game1.ScreenHeight - 40),
                Input = new Input()
                {
                    Left = Keys.Left,
                    Right = Keys.Right
                }
            };
            _ball = new Ball(ballTexture)
            {
                Position = new Vector2((Game1.ScreenWidth / 2) - (ballTexture.Width / 2), (Game1.ScreenHeight / 2) - (ballTexture.Height / 2))
            };

            LoadBlocks();

        }

        private void LoadBlocks()
        {
            var blockTexture = _content.Load<Texture2D>("block");
            _blocks = new List<Block>();
            // Add 10 block in a random pattern
            foreach (var _ in Enumerable.Range(0, 10))
            {
                var block = new Block(blockTexture, 10);

                // Randomise block position
                var blockX = _random.Next(0, Game1.ScreenWidth - block.Rectangle.Width);
                var ylimit = (Game1.ScreenHeight / 2) - block.Rectangle.Height; // Limit Y to top half of screen
                var blockY = _random.Next(0, ylimit);

                block.Position = new Vector2(blockX, blockY);

                _blocks.Add(block);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!_levelComplete)
            {
                _paddle.Update(gameTime);
                _ball.Update(gameTime, _blocks, _paddle);
                foreach (var block in _blocks)
                {
                    block.Update(gameTime);
                }

                UpdateScore();

                PostUpdate();

                if (!_blocks.Any())
                {
                    OnLevelComplete();
                }
            }

            PostUpdate();
        }

        private void UpdateScore()
        {
            foreach (var block in _blocks)
            {
                if (block.IsRemoved)
                {
                    LevelScore += block._pointValue;
                }
            }
        }

        private void PostUpdate()
        {
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i].IsRemoved)
                {
                    _blocks.RemoveAt(i);
                    i--;
                }
            }
        }

        private void OnLevelComplete()
        {
            _levelComplete = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            _paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);

            foreach (var block in _blocks)
            {
                block.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            _content.Unload();
        }
    }
}
