using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget
{
    public class ScrapMetal : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.None;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D scrapMetalTexture;

        public static ScrapMetal NewScrapMetal(Vector2 position)
        {
            ScrapMetal scrapMetal = new ScrapMetal();
            scrapMetal.position = position;
            scrapMetal.hitbox = new Rectangle((int)position.X, (int)position.Y, 10, 11);
            return scrapMetal;
        }

        public override void Update()
        {
            DetectCollisions(Main.entitiesList);
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            Main.playerScrap += 1;
            Main.collectedScrap += 1;
            DestroyInstance();
        }

        public new void DestroyInstance()
        {
            Map.generatedMapObjects[Map.activeMapIndex].Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(scrapMetalTexture, position - Main.cameraPosition, null, Color.White);
        }
    }
}
