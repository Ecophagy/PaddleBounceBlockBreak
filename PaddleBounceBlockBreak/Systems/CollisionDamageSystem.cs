using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class CollisionDamageSystem
{
    public void Update(HealthComponent health, CollisionComponent collision, RenderComponent render)
    {
        if (collision.Impact)
        {
            health.Health--;
            if (health.Health > 0)
            {
                // Health <= 0 will be deleted, so there is no associated texture
                render.Texture = health.DamageTextures[health.Health];
            }

            collision.Impact = false;
        }
    }
}