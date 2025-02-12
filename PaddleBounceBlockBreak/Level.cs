using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Constants;
using PaddleBounceBlockBreak.Models;
using PaddleBounceBlockBreak.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using PaddleBounceBlockBreak.Components;
using PaddleBounceBlockBreak.Entities;
using PaddleBounceBlockBreak.Systems;

namespace PaddleBounceBlockBreak
{
    public class Level : IDisposable
    {
        // Entities in the level
        private Ball _ball;
        private Paddle _paddle;
        private List<Block> _blocks;

        // Component lists
        private Dictionary<Guid, RenderComponent> _renderComponents = new();
        private Dictionary<Guid, PositionComponent> _positionComponents = new();
        private Dictionary<Guid, MotionComponent> _motionComponents = new();
        private Dictionary<Guid, UserControlComponent> _userControlComponents = new();
        
        // Systems
        private readonly RenderSystem _renderSystem = new();
        private readonly UserInputSystem _userInputSystem = new();
        private readonly PhysicsSystem _physicsSystem = new();

        // Level State
        private Random _random = new Random();
        public LevelState LevelState { get; private set; }
        public int LevelScore { get; private set; } = 0;

        // Level Content
        private ContentManager _content;

        public Level(IServiceProvider serviceProvider) // TODO: Can set difficulty/number of blocks here
        {
            _content = new ContentManager(serviceProvider, "Content");
           
            // Ball
            var ballEntity = new Entity("ball");
            var ballTexture = _content.Load<Texture2D>("ball");
            _renderComponents.Add(ballEntity.EntityId, new RenderComponent(ballTexture));
            _positionComponents.Add(ballEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (ballTexture.Width / 2), (Game1.ScreenHeight / 2) - (ballTexture.Height / 2))));
            _motionComponents.Add(ballEntity.EntityId, new MotionComponent(3f, new Vector2(3,3))); // TODO: Set initial velocity to 0
            
            // Paddle
            var paddleEntity = new Entity("paddle");
            var paddleTexture = _content.Load<Texture2D>("paddle");
            _renderComponents.Add(paddleEntity.EntityId, new RenderComponent(paddleTexture));
            _positionComponents.Add(paddleEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (paddleTexture.Width / 2), Game1.ScreenHeight - 40)));
            _motionComponents.Add(paddleEntity.EntityId, new MotionComponent(5f, new Vector2()));
            _userControlComponents.Add(paddleEntity.EntityId, new UserControlComponent(new Input()
            {
                Left = Keys.Left,
                Right = Keys.Right
            }));
            
            // Blocks
            var blockTexture = _content.Load<Texture2D>("block");
            foreach (var i in Enumerable.Range(0, 10))
            {
                var blockEntity = new Entity($"block_{i}");
                _renderComponents.Add(blockEntity.EntityId, new RenderComponent(blockTexture));
                // Randomise block position
                var blockX = _random.Next(0, Game1.ScreenWidth - blockTexture.Width);
                var ylimit = (Game1.ScreenHeight / 2) - blockTexture.Height; // Limit Y to top half of screen
                var blockY = _random.Next(0, ylimit);
                _positionComponents.Add(blockEntity.EntityId, new PositionComponent(new Vector2(blockX, blockY)));
            }
            
            LevelState = LevelState.LEVEL_ACTIVE;

/*
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



            LoadBlocks();*/

        }

        private void LoadBlocks()
        {
            var blockTexture = _content.Load<Texture2D>("block");
            var damagedBlockTexture = _content.Load<Texture2D>("damaged_block");

            // Dictionary of block textures that change as they take hits
            var blockTextures = new Dictionary<int, Texture2D>
            {
                { 1, damagedBlockTexture },
                { 2, blockTexture },
            };

            _blocks = new List<Block>();
            // Add 10 block in a random pattern
            foreach (var _ in Enumerable.Range(0, 10))
            {
                var block = new Block(blockTextures, 10, 2);

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
            if (LevelState == LevelState.LEVEL_ACTIVE)
            {
                foreach (var component in _userControlComponents)
                {
                    _userInputSystem.Update(component.Value, _motionComponents[component.Key]);
                }

                foreach (var component in _motionComponents)
                {
                    _physicsSystem.Update(component.Value, _positionComponents[component.Key]);
                }
            }

            /*
            if (LevelState == LevelState.LEVEL_ACTIVE)
            {
                _paddle.Update(gameTime);
                _ball.Update(gameTime);

                // Handle Ball collisions with other objects
                _ = _ball.HandleSpriteCollision(_paddle);
                foreach (var block in _blocks)
                {
                    if(_ball.HandleSpriteCollision(block))
                    {
                        block.OnHit();
                        // Update score
                        LevelScore += block._pointValue;
                    }
                }
                var levelFail = _ball.HandleWallCollision(Game1.ScreenWidth, Game1.ScreenHeight);
                if (levelFail)
                {
                    OnLevelFail();
                }

                foreach (var block in _blocks)
                {
                    block.Update(gameTime);
                }

                PostUpdate();

                if (!_blocks.Any())
                {
                    OnLevelComplete();
                }
            }

            PostUpdate();
            */
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

        public void Reset()
        {
            _ball.Restart();
            LevelState = LevelState.LEVEL_ACTIVE;
        }

        private void OnLevelComplete()
        {
            LevelState = LevelState.LEVEL_COMPLETE;
        }

        private void OnLevelFail()
        {
            LevelState = LevelState.LEVEL_FAIL;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var components in _renderComponents)
            {
                _renderSystem.Draw(spriteBatch, components.Value, _positionComponents[components.Key]);
            }

            // _paddle.Draw(spriteBatch);
            // _ball.Draw(spriteBatch);
            //
            // foreach (var block in _blocks)
            // {
            //     block.Draw(spriteBatch);
            // }
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
