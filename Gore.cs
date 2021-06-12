using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget
{
    public class Gore
    {
        public static Texture2D[] goreTextues;

        public const int TrashGore = 0;
        public const int GarbageGore = 1;

        private int goreType = 0;
        private Vector2 gorePosition;

        public static Gore NewGore(int goreType, Vector2 position)
        {
            Gore gore = new Gore();
            gore.goreType = goreType;
            gore.gorePosition = position;
            return gore;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(goreTextues[goreType], gorePosition - Main.cameraPosition, null, Color.White);
        }
    }
}
