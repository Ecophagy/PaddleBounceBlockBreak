using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PaddleBounceBlockBreak.Sprites
{
    public class Block : Sprite
    {
        public int _pointValue { get; private set; }

        public Block(Texture2D texture, int pointValue) : base(texture)
        {
            _pointValue = pointValue;

            // Blocks don't move
            Speed = 0f; 
            Velocity = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == this)
                {
                    continue;
                }

                // TODO: Check only for ball sprite?
                if (this.IsTouchingTop(sprite) || 
                    this.IsTouchingRight(sprite) ||
                    this.IsTouchingBottom(sprite) || 
                    this.IsTouchingLeft(sprite))
                {
                    // Something (the ball) touched the block, so break it
                    this.IsRemoved = true;
                }
            }
        }

    }
}
