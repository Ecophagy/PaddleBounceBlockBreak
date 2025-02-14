using Microsoft.Xna.Framework;
using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class EntityCollisionSystem
{
     public void Update(CollisionComponent collision, PositionComponent activePosition, MotionComponent activeMotion, Components.CollisionComponent passiveCollision, PositionComponent passivePosition)
     {
          var activeRect = new Rectangle((int)activePosition.Position.X, (int)activePosition.Position.Y, collision.Width, collision.Height);
          var passiveRect = new Rectangle((int)passivePosition.Position.X, (int)passivePosition.Position.Y, passiveCollision.Width, passiveCollision.Height);

          // Update Velocity on collisions with other sprite
          if (activeMotion.Velocity.X > 0 && IsTouchingLeft(activeMotion.Velocity, activeRect, passiveRect))
          {
               activeMotion.Velocity = activeMotion.Velocity with { X = -activeMotion.Velocity.X };
               passiveCollision.Impact = true;
          }
          if (activeMotion.Velocity.X < 0 && IsTouchingRight(activeMotion.Velocity, activeRect, passiveRect))
          {
               activeMotion.Velocity = activeMotion.Velocity with { X = -activeMotion.Velocity.X };
               passiveCollision.Impact = true;
          }
          if (activeMotion.Velocity.Y > 0 && IsTouchingTop(activeMotion.Velocity, activeRect, passiveRect))
          {
               activeMotion.Velocity = activeMotion.Velocity with { Y = -activeMotion.Velocity.Y };
               passiveCollision.Impact = true;
          }
          if (activeMotion.Velocity.Y < 0 && IsTouchingBottom(activeMotion.Velocity, activeRect, passiveRect))
          {
               activeMotion.Velocity = activeMotion.Velocity with { Y = -activeMotion.Velocity.Y };
               passiveCollision.Impact = true;
          }
     }
     
     private bool IsTouchingLeft(Vector2 activeVelocity, Rectangle active, Rectangle passive)
     {
          return active.Right + activeVelocity.X > passive.Left &&
                 active.Left < passive.Left &&
                 active.Bottom > passive.Top &&
                 active.Top < passive.Bottom;
     }

     private bool IsTouchingRight(Vector2 activeVelocity, Rectangle active, Rectangle passive)
     {
          return active.Left + activeVelocity.X < passive.Right &&
                 active.Right > passive.Right &&
                 active.Bottom > passive.Top &&
                 active.Top < passive.Bottom;
     }

     private bool IsTouchingTop(Vector2 activeVelocity, Rectangle active, Rectangle passive)
     {
          return active.Bottom + activeVelocity.Y > passive.Top &&
                 active.Top < passive.Top &&
                 active.Right > passive.Left &&
                 active.Left < passive.Right;
     }

     private bool IsTouchingBottom(Vector2 activeVelocity, Rectangle active, Rectangle passive)
     {
          return active.Top + activeVelocity.Y < passive.Bottom &&
                 active.Bottom > passive.Bottom &&
                 active.Right > passive.Left &&
                 active.Left < passive.Right;
     }
}