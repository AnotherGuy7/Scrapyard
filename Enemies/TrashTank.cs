
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Tight_Budget.Projectiles;

namespace Tight_Budget.Enemies
{
    public class TrashTank : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };
        public override int startingHealth => 4;

        public static Texture2D trashTankSpritesheet;
        public static Texture2D trashTankBarrelTexture;
        public static SoundEffect trashTankHitSound;
        public static SoundEffect trashTankShootSound;

        public const float MoveSpeed = 0.2f;
        public const int TrashTankWidth = 13;
        public const int TrashTankHeight = 12;
        public const float ShootRange = 75f;

        private int health = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;
        private Color drawColor;
        private int colorChangeTimer = 0;
        private Direction direction;
        private float barrelRotation = 0f;
        private int shootTimer = 0;

        private enum Direction      //This is dependent on the spritesheet
        {
            Front,
            Left,
            Back,
            Right
        }

        public static TrashTank NewTrashTank(Vector2 position)
        {
            TrashTank trashTank = new TrashTank();
            trashTank.health = trashTank.startingHealth;
            trashTank.position = position;
            trashTank.hitbox = new Rectangle((int)position.X, (int)position.Y, TrashTankWidth, TrashTankHeight);
            return trashTank;
        }

        public override void Update()
        {
            DetectCollisions(Main.entitiesList);
            Vector2 velocity = Main.player.position - position;
            velocity.Normalize();
            Vector2 barrelRotationVector = velocity;
            velocity *= MoveSpeed;

            if (Vector2.Distance(Main.player.playerCenter, position) <= ShootRange)
            {
                velocity = Vector2.Zero;

                shootTimer++;
                if (shootTimer > 3 * 60)
                {
                    shootTimer = 0;
                    trashTankShootSound.Play();
                    TrashTankBullet.NewTrashTankBullet(center + (barrelRotationVector * 10f), barrelRotationVector * 4f);
                }
            }

            int xDirection = 1;
            if (velocity.X < 0)
                xDirection = -1;

            int yDirection = 1;
            if (velocity.Y < 0)
                yDirection = -1;

            if (DetectTileCollisions(center + new Vector2(9f * xDirection, 0f)))
                velocity.X = 0f;

            if (DetectTileCollisions(center + new Vector2(0f, 6f * yDirection)))
                velocity.Y = 0f;

            position += velocity;
            center = position + new Vector2(TrashTankWidth / 2f, TrashTankHeight / 2f);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (velocity != Vector2.Zero)
            {
                float angle = MathHelper.ToDegrees((float)(Math.Atan2(velocity.Y, velocity.X) + Math.PI));
                if (angle <= 45f || angle > 315f)
                {
                    direction = Direction.Left;
                }
                else if (angle > 45f && angle <= 135f)
                {
                    direction = Direction.Back;
                }
                else if (angle > 135f && angle <= 225f)
                {
                    direction = Direction.Right;
                }
                else if (angle > 225 && angle <= 315f)
                {
                    direction = Direction.Front;
                }
            }

            barrelRotation = (float)(Math.Atan2(barrelRotationVector.Y, barrelRotationVector.X)) - MathHelper.ToRadians(90f);

            frameCounter++;
            if (frameCounter >= 3)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 2)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, TrashTankHeight * (frame + (int)direction * 2), TrashTankWidth, TrashTankHeight);
            }

            if (colorChangeTimer > 0)
            {
                drawColor = Color.Red;
                colorChangeTimer--;
            }
            else
            {
                drawColor = Color.White;
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            health -= 1;
            colorChangeTimer += 5;
            if (health <= 0)
            {
                DestroyInstance();
                Map.generatedMapObjects[Map.activeMapIndex].Remove(this);
                Map.generatedMapObjects[Map.activeMapIndex].Add(Gore.NewGore(Gore.TankGore, center));
            }
            trashTankHitSound.Play();
            collider.DestroyInstance();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(trashTankSpritesheet, position - Main.cameraPosition, animRect, drawColor);

            spriteBatch.Draw(trashTankBarrelTexture, center - Main.cameraPosition, null, Color.White, barrelRotation, new Vector2(3f, 0f), 1f, SpriteEffects.None, 0f);
        }
    }
}
