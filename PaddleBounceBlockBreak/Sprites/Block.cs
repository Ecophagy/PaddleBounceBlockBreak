using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public override void Update(GameTime gameTime)
        {
            // Do nothing, for now
        }

        public void BlockHit()
        {
            this.IsRemoved = true;
        }

    }
}
