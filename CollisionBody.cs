using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Tight_Budget
{
    public abstract class CollisionBody
    {
        public Vector2 position;
        public Rectangle hitbox;

        public virtual CollisionType[] colliderTypes { get; }

        public virtual CollisionType collisionType { get; }

        public enum CollisionType
        {
            None,
            Player,
            Enemies,
            FriendlyProjectiles,
            EnemyProjectiles
        }

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public void DetectCollisions(List<CollisionBody> possibleIntersectors)
        {
            CollisionBody[] intersectersCopy = possibleIntersectors.ToArray();
            foreach (CollisionBody intersector in intersectersCopy)
            {
                if (hitbox.Intersects(intersector.hitbox))
                {
                    for (int c = 0; c < colliderTypes.Length; c++)
                    {
                        if (colliderTypes[c] == intersector.collisionType)
                        {
                            HandleCollisions(intersector, intersector.collisionType);
                            break;
                        }
                    }
                }
            }
        }

        public bool DetectTileCollisions(Vector2 position)
        {
            Point positionPoint = (position / 16).ToPoint();
            if (positionPoint.X > Map.activeMap.GetLength(0) || positionPoint.Y > Map.activeMap.GetLength(1))
                return false;

            if (Map.activeMap[positionPoint.X, positionPoint.Y].collisionStyle == Tile.CollisionStyle.Solid)
                return true;

            return false;
        }

        public virtual void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        { }

        public void DestroyInstance()
        {
            Main.entitiesList.Remove(this);
        }
    }
}
