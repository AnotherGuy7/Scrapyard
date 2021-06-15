using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget
{
    public class Tile
    {
        /*public static Texture2D grassTexture;
        public static Texture2D grassyGrassTexture;
        public static Texture2D concreteTexture;*/
        public static Texture2D groundTexture;
        public static Texture2D groundWithARockTexture;
        public static Texture2D groundWithAWeirdHoleTexture;
        public static Texture2D scrapTexture;
        public static Texture2D scrapTopTexture;
        public static Texture2D entranceDirectionalArrowTexture;
        public static Texture2D exitDirectionalArrowTexture;

        public static Texture2D oldTiresTexture;
        public static Texture2D garbagePile;

        public Vector2 position;
        public Texture2D texture;
        public float rotation;
        public CollisionStyle collisionStyle;

        public enum CollisionStyle
        {
            None,
            Solid
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Vector2.Distance(position, Main.player.position) >= (Main.halfScreenWidth * 2f) + 50f)
                return;

            Vector2 tileOrigin = new Vector2(8f, 8f);
            spriteBatch.Draw(texture, position - Main.cameraPosition + tileOrigin, null, Color.White, MathHelper.ToRadians(rotation), tileOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
