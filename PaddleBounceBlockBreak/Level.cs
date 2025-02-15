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
        
        private readonly Entity _ballEntity = new Entity("ball"); 
        private readonly Entity _paddleEntity = new Entity("paddle");

        // Component lists
        private Dictionary<Guid, RenderComponent> _renderComponents = new();
        private Dictionary<Guid, PositionComponent> _positionComponents = new();
        private Dictionary<Guid, MotionComponent> _motionComponents = new();
        private Dictionary<Guid, AccelerationComponent> _accelerationComponents = new();
        private Dictionary<Guid, UserControlComponent> _userControlComponents = new();
        private Dictionary<Guid, CollisionComponent> _collisionComponents = new();
        private Dictionary<Guid, HealthComponent> _healthComponents = new();
        
        // Systems
        private readonly RenderSystem _renderSystem = new();
        private readonly UserInputSystem _userInputSystem = new();
        private readonly MotionSystem _motionSystem = new();
        private readonly AccelerationSystem _accelerationSystem = new();
        private readonly EntityCollisionSystem _entityCollisionSystem = new();
        private readonly WallCollisionSystem _wallCollisionSystem = new();
        private readonly CollisionDamageSystem _collisionDamageSystem = new();

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
            _renderComponents.Add(_ballEntity.EntityId, new RenderComponent(ballTexture));
            _positionComponents.Add(_ballEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (ballTexture.Width / 2), (Game1.ScreenHeight / 2) - (ballTexture.Height / 2))));
            _motionComponents.Add(_ballEntity.EntityId, new MotionComponent(3f, new Vector2(-3,-3))); // TODO: Set initial velocity to 0
            _collisionComponents.Add(_ballEntity.EntityId, new CollisionComponent(ballTexture.Height, ballTexture.Width));
            _accelerationComponents.Add(_ballEntity.EntityId, new AccelerationComponent(new Vector2(1.5f, 1.5f)));
            
            // Paddle
            var paddleTexture = _content.Load<Texture2D>("paddle");
            _renderComponents.Add(_paddleEntity.EntityId, new RenderComponent(paddleTexture));
            _positionComponents.Add(_paddleEntity.EntityId, new PositionComponent(new Vector2((Game1.ScreenWidth / 2) - (paddleTexture.Width / 2), Game1.ScreenHeight - 40)));
            _motionComponents.Add(_paddleEntity.EntityId, new MotionComponent(5f, new Vector2()));
            _userControlComponents.Add(_paddleEntity.EntityId, new UserControlComponent(new Input()
            {
                Left = Keys.Left,
                Right = Keys.Right
            }));
            _collisionComponents.Add(_paddleEntity.EntityId, new CollisionComponent(paddleTexture.Height, paddleTexture.Width));
            
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
                _renderComponents.Add(blockEntity.EntityId, new RenderComponent(blockTexture));
                // Randomise block position
                var blockX = _random.Next(0, Game1.ScreenWidth - blockTexture.Width);
                var ylimit = (Game1.ScreenHeight / 2) - blockTexture.Height; // Limit Y to top half of screen
                var blockY = _random.Next(0, ylimit);
                _positionComponents.Add(blockEntity.EntityId, new PositionComponent(new Vector2(blockX, blockY)));
                _collisionComponents.Add(blockEntity.EntityId, new CollisionComponent(blockTexture.Height, blockTexture.Width));
                _healthComponents.Add(blockEntity.EntityId, new HealthComponent(2, blockTextures));
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
            if (LevelState == LevelState.LEVEL_ACTIVE)
            {
                foreach (var (entityId, component) in _userControlComponents)
                {
                    _userInputSystem.Update(component, _motionComponents[entityId]);
                }
                
                // Handles collision between ball and other objects
                foreach (var (entityId, component) in _collisionComponents)
                {
                    if (entityId != _ballEntity.EntityId)
                    {
                        _entityCollisionSystem.Update(_collisionComponents[_ballEntity.EntityId],
                            _positionComponents[_ballEntity.EntityId],
                            _motionComponents[_ballEntity.EntityId],
                            component,
                            _positionComponents[entityId]);
                    }
                }
                
                // Handle ball collision with walls
                _wallCollisionSystem.Update(_collisionComponents[_ballEntity.EntityId], 
                    _positionComponents[_ballEntity.EntityId],
                    _motionComponents[_ballEntity.EntityId], 
                    Game1.ScreenWidth,
                    Game1.ScreenHeight);

                foreach (var (entityId, component) in _healthComponents)
                {
                    _collisionDamageSystem.Update(component, _collisionComponents[entityId], _renderComponents[entityId]);
                }

                foreach (var (entityId, component) in _accelerationComponents)
                {
                    _accelerationSystem.Update(gameTime, component, _motionComponents[entityId]);
                }
                
                foreach (var (entityId, component) in _motionComponents)
                {
                    _motionSystem.Update(component, _positionComponents[entityId]);
                }
                
                PostUpdate();
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
            foreach (var (entityId, component) in _healthComponents)
            {
                if (component.Health <= 0)
                {
                    // FIXME: Not very scalable, might be worth having a list of component lists
                    // That we can iterate over to find elements that need removing
                    _renderComponents.Remove(entityId);
                    _positionComponents.Remove(entityId);
                    _collisionComponents.Remove(entityId);
                    _healthComponents.Remove(entityId);
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
            foreach (var (entityId, component) in _renderComponents)
            {
                _renderSystem.Draw(spriteBatch, component, _positionComponents[entityId]);
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
