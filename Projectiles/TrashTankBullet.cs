using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tight_Budget.Projectiles
{
    public class TrashTankBullet : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.EnemyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        public static Texture2D trashTankBulletTexture;

        public const int TrashTankBulletWidth = 5;
        public const int TrashTankBulletHeight = 5;

        private Vector2 velocity;
        private float bulletRotation;
        private float lifeTimer = 0;

        public static void NewTrashTankBullet(Vector2 position, Vector2 velocity)
        {
            TrashTankBullet trashTankBullet = new TrashTankBullet();
            trashTankBullet.position = position;
            trashTankBullet.velocity = velocity;
            trashTankBullet.bulletRotation = (float)Math.Atan2(velocity.Y, velocity.X) + MathHelper.ToRadians(90f);
            trashTankBullet.hitbox = new Rectangle((int)position.X, (int)position.Y, TrashTankBulletWidth, TrashTankBulletHeight);
            Main.entitiesList.Add(trashTankBullet);
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
            Vector2 bulletOrigin = new Vector2(TrashTankBulletWidth / 2f, TrashTankBulletHeight / 2f);
            spriteBatch.Draw(trashTankBulletTexture, position - Main.cameraPosition, null, Color.White, bulletRotation, bulletOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
