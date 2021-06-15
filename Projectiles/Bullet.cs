using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tight_Budget.Projectiles
{
    public class Bullet : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static Texture2D bulletTexture;

        public const int BulletWidth = 1;
        public const int BulletHeight = 7;

        private Vector2 velocity;
        private float bulletRotation;
        private float lifeTimer = 0;

        public static void NewBullet(Vector2 position, Vector2 velocity)
        {
            Bullet bullet = new Bullet();
            bullet.position = position;
            bullet.velocity = velocity;
            bullet.bulletRotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.ToRadians(90f);
            bullet.hitbox = new Rectangle((int)position.X, (int)position.Y, BulletWidth, BulletHeight);
            Main.entitiesList.Add(bullet);
        }

        public override void Update()
        {
            lifeTimer++;
            if (lifeTimer >= 10 * 60)
            {
                DestroyInstance();
            }
            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 bulletOrigin = new Vector2(BulletWidth / 2f, BulletHeight / 2f);
            spriteBatch.Draw(bulletTexture, position - Main.cameraPosition, null, Color.White, bulletRotation, bulletOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
