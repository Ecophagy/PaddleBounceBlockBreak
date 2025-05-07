using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class RenderTextSystem
{
    public void Draw(SpriteBatch spriteBatch, TextRenderComponent renderText, PositionComponent positionComponent)
    {
        spriteBatch.DrawString(renderText.Font, 
            $"{renderText.BaseText} {renderText.Text}", 
            positionComponent.Position, 
            Color.White);
    }
}