using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tight_Budget.Enemies
{
    public class TheTrash : Enemy
    {
        public static Texture2D trashTexture;

        private float floatTimer;

        public override void Update()
        {
            floatTimer += 0.058105f;

            position.Y += (float)Math.Sin(floatTimer);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(trashTexture, position - Main.cameraPosition, null, Color.White);
        }
    }
}
