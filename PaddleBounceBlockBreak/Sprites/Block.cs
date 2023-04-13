using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PaddleBounceBlockBreak.Sprites
{
    public class Block : Sprite
    {
        public int _pointValue { get; private set; }
        private int _health; // Hits a block can take before being broken

        // Dictionary of block health remaining and corresponding texture
        private Dictionary<int, Texture2D> _blockTextures;

        public Block(Dictionary<int, Texture2D> textures, int pointValue, int startingHealth) : base(textures[startingHealth])
        {
            _pointValue = pointValue;
            _health = startingHealth;
            _blockTextures = textures;

            // Blocks don't move
            Speed = 0f; 
            Velocity = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            // Do nothing, for now
        }

        public void OnHit()
        {
            _health--;
            if (_health <= 0)
            {
                this.IsRemoved = true;
            }
            else
            {
                _texture = _blockTextures[_health];
            }
        }

    }
}
