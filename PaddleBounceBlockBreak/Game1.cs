using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaddleBounceBlockBreak.Constants;
using PaddleBounceBlockBreak.HUD;
using System;
using System.Collections.Generic;
using PaddleBounceBlockBreak.Components;
using PaddleBounceBlockBreak.Entities;
using PaddleBounceBlockBreak.Systems;

namespace PaddleBounceBlockBreak
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // non-static?
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static Random Random;

        private Level _level;
        private DynamicHudText _scoreOverlay;
        private int _totalScore;
        private int _lives;
        private GameState _gameState;

        private HudText _gameOverOverlay;
        private DynamicHudText _livesOverlay;
        
        // Entities
        private readonly Entity GameEntity = new Entity("game");
        private readonly Entity ScoreEntity = new Entity("score");
        private readonly Entity LivesEntity = new Entity("lives");
        private readonly Entity GameOverEntity = new Entity("gameOver");
        
        // Systems
        private readonly RenderSystem _renderSystem = new();
        private readonly RenderTextSystem _renderTextSystem = new();
        private readonly UserInputSystem _userInputSystem = new();
        private readonly MotionSystem _motionSystem = new();
        private readonly AccelerationSystem _accelerationSystem = new();
        private readonly EntityCollisionSystem _entityCollisionSystem = new();
        private readonly WallCollisionSystem _wallCollisionSystem = new();
        private readonly CollisionDamageSystem _collisionDamageSystem = new();
        private readonly ScoreSystem _scoreSystem = new();
        
        // Component Lists
        private Dictionary<Guid, TextRenderComponent> TextRenderComponents = new();
        private Dictionary<Guid, PositionComponent> GamePositionComponents = new(); // Gross???
        private Dictionary<Guid, TotalScoreComponent> TotalScoreComponents = new();
        

        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ScreenWidth = _graphics.PreferredBackBufferWidth;
            ScreenHeight = _graphics.PreferredBackBufferHeight;
            Random = new Random();

            _lives = 3; // Start with 3 lives
            _gameState = GameState.GAME_ACTIVE;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _scoreOverlay = new DynamicHudText(Content.Load<SpriteFont>("HudFont"), "Score: ", new Vector2(10, 0));
            _livesOverlay = new DynamicHudText(Content.Load<SpriteFont>("HudFont"), "Lives: ", new Vector2(10, _scoreOverlay.Size.Y));
            _gameOverOverlay = new CentredHudText(Content.Load<SpriteFont>("HudFont"), "Game Over!", new Vector2(ScreenWidth/2, ScreenHeight/2));

            // Score
            TotalScoreComponents.Add(GameEntity.EntityId, new TotalScoreComponent());
            
            var scoreFont = Content.Load<SpriteFont>("HudFont");
            TextRenderComponents.Add(ScoreEntity.EntityId, new TextRenderComponent(scoreFont, "Score: "));
            GamePositionComponents.Add(ScoreEntity.EntityId, new PositionComponent(new Vector2(10, 0)));
            
            // Lives
            var livesFont = Content.Load<SpriteFont>("HudFont");
            TextRenderComponents.Add(LivesEntity.EntityId, new TextRenderComponent(livesFont, "Lives:"));
            GamePositionComponents.Add(LivesEntity.EntityId, new PositionComponent(new Vector2(10, TextRenderComponents[ScoreEntity.EntityId].Size.Y)));
            
            // Game Over
            // TODO
            
            LoadNextLevel();
        }

        private void LoadNextLevel()
        {
            if (_level != null)
            {
                _totalScore += _level.LevelScore;
                _level.Dispose();
            }

            _level = new Level(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (_gameState == GameState.GAME_ACTIVE)
            {
                if (_level.LevelState == LevelState.LEVEL_ACTIVE)
                {
                    foreach (var (entityId, component) in _level.UserControlComponents)
                    {
                        _userInputSystem.Update(component, _level.MotionComponents[entityId]);
                    }

                    // Handles collision between ball and other objects
                    foreach (var (entityId, component) in _level.CollisionComponents)
                    {
                        if (entityId != _level.BallEntity.EntityId)
                        {
                            _entityCollisionSystem.Update(_level.CollisionComponents[_level.BallEntity.EntityId],
                                _level.PositionComponents[_level.BallEntity.EntityId],
                                _level.MotionComponents[_level.BallEntity.EntityId],
                                component,
                                _level.PositionComponents[entityId]);
                        }
                    }

                    // Handle ball collision with walls
                    _wallCollisionSystem.Update(_level.CollisionComponents[_level.BallEntity.EntityId],
                        _level.PositionComponents[_level.BallEntity.EntityId],
                        _level.MotionComponents[_level.BallEntity.EntityId],
                        Game1.ScreenWidth,
                        Game1.ScreenHeight);

                    foreach (var (entityId, component) in _level.HealthComponents)
                    {
                        _collisionDamageSystem.Update(component, _level.CollisionComponents[entityId],
                            _level.RenderComponents[entityId]);
                    }

                    foreach (var (entityId, component) in _level.AccelerationComponents)
                    {
                        _accelerationSystem.Update(gameTime, component, _level.MotionComponents[entityId]);
                    }

                    foreach (var (entityId, component) in _level.MotionComponents)
                    {
                        _motionSystem.Update(component, _level.PositionComponents[entityId]);
                    }

                    foreach (var (entityId, component) in _level.ScoreComponents)
                    {
                        _scoreSystem.Update(TotalScoreComponents[GameEntity.EntityId], 
                            _level.HealthComponents[entityId],
                            _level.ScoreComponents[entityId],
                            TextRenderComponents[ScoreEntity.EntityId]);
                    }

                    PostUpdate();
                }

                if (_level.LevelState == LevelState.LEVEL_FAIL)
                {
                    _lives -= 1;
                    if (_lives == 0)
                    {
                        _gameState = GameState.GAME_OVER;
                    }
                    else
                    {
                        _level.Reset();
                    }
                }

                if (_level.LevelState == LevelState.LEVEL_COMPLETE)
                {
                    LoadNextLevel();
                }
            }

            base.Update(gameTime);
        }


        private void PostUpdate()
        {
            foreach (var (entityId, component) in _level.HealthComponents)
            {
                if (component.Health <= 0)
                {
                    // FIXME: Not very scalable, might be worth having a list of component lists
                    // That we can iterate over to find elements that need removing
                    _level.RenderComponents.Remove(entityId);
                    _level.PositionComponents.Remove(entityId);
                    _level.CollisionComponents.Remove(entityId);
                    _level.HealthComponents.Remove(entityId);
                    _level.ScoreComponents.Remove(entityId);
                }
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            _spriteBatch.Begin();

            foreach (var (entityId, component) in _level.RenderComponents)
            {
                _renderSystem.Draw(_spriteBatch, component, _level.PositionComponents[entityId]);
            }
            
            foreach (var (entityId, component) in TextRenderComponents)
            {
                _renderTextSystem.Draw(_spriteBatch, component, GamePositionComponents[entityId]);
            }

            // _scoreOverlay.Draw(_spriteBatch, (_totalScore + _level.LevelScore).ToString());
            // _livesOverlay.Draw(_spriteBatch, _lives.ToString());

            if (_gameState == GameState.GAME_OVER)
            {
                // TODO
                _gameOverOverlay.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    
}