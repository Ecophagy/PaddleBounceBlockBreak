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
        
        public readonly Entity BallEntity = new Entity("ball"); 
        public readonly Entity PaddleEntity = new Entity("paddle");

        // Component lists
        public Dictionary<Guid, RenderComponent> RenderComponents = new();
        public Dictionary<Guid, PositionComponent> PositionComponents = new();
        public Dictionary<Guid, MotionComponent> MotionComponents = new();
        public Dictionary<Guid, AccelerationComponent> AccelerationComponents = new();
        public Dictionary<Guid, UserControlComponent> UserControlComponents = new();
        public Dictionary<Guid, CollisionComponent> CollisionComponents = new();
        public Dictionary<Guid, HealthComponent> HealthComponents = new();

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
            var ballTexture = _content.Load<Texture2D>("ball");
            RenderComponents.Add(BallEntity.EntityId, new RenderComponent(ballTexture));
            PositionComponents.Add(BallEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (ballTexture.Width / 2), (Game1.ScreenHeight / 2) - (ballTexture.Height / 2))));
            MotionComponents.Add(BallEntity.EntityId, new MotionComponent(3f, new Vector2(-3,-3))); // TODO: Set initial velocity to 0
            CollisionComponents.Add(BallEntity.EntityId, new CollisionComponent(ballTexture.Height, ballTexture.Width));
            AccelerationComponents.Add(BallEntity.EntityId, new AccelerationComponent(new Vector2(1.5f, 1.5f)));
            
            // Paddle
            var paddleTexture = _content.Load<Texture2D>("paddle");
            RenderComponents.Add(PaddleEntity.EntityId, new RenderComponent(paddleTexture));
            PositionComponents.Add(PaddleEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (paddleTexture.Width / 2), Game1.ScreenHeight - 40)));
            MotionComponents.Add(PaddleEntity.EntityId, new MotionComponent(5f, new Vector2()));
            UserControlComponents.Add(PaddleEntity.EntityId, new UserControlComponent(new Input()
            {
                Left = Keys.Left,
                Right = Keys.Right
            }));
            CollisionComponents.Add(PaddleEntity.EntityId, new CollisionComponent(paddleTexture.Height, paddleTexture.Width));
            
            // Blocks
            var blockTexture = _content.Load<Texture2D>("block");
            var damagedBlockTexture = _content.Load<Texture2D>("damaged_block");

            // Dictionary of block textures that change as they take hits
            var blockTextures = new Dictionary<int, Texture2D>
            {
                { 1, damagedBlockTexture },
                { 2, blockTexture },
            };
            foreach (var i in Enumerable.Range(0, 10))
            {
                var blockEntity = new Entity($"block_{i}");
                RenderComponents.Add(blockEntity.EntityId, new RenderComponent(blockTexture));
                // Randomise block position
                var blockX = _random.Next(0, Game1.ScreenWidth - blockTexture.Width);
                var ylimit = (Game1.ScreenHeight / 2) - blockTexture.Height; // Limit Y to top half of screen
                var blockY = _random.Next(0, ylimit);
                PositionComponents.Add(blockEntity.EntityId, new PositionComponent(new Vector2(blockX, blockY)));
                CollisionComponents.Add(blockEntity.EntityId, new CollisionComponent(blockTexture.Height, blockTexture.Width));
                HealthComponents.Add(blockEntity.EntityId, new HealthComponent(2, blockTextures));
            }
            
            LevelState = LevelState.LEVEL_ACTIVE;
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
