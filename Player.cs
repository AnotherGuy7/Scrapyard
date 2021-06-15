using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Tight_Budget.Enemies;
using Tight_Budget.Projectiles;

namespace Tight_Budget
{
    public class Player : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.Player;
        public override CollisionType[] colliderTypes => new CollisionType[2] { CollisionType.Enemies, CollisionType.EnemyProjectiles };

        public static Texture2D[] playerWalkingSpritesheets;
        public static Texture2D[] playerIdleSpritesheets;
        public static Texture2D[] playerGunTextures;
        public static SoundEffect[] gunShotSounds;

        public const float moveSpeed = 1.3f;
        public const int PlayerWidth = 13;
        public const int PlayerHeight = 25;
        public const float GunRadius = 18f;

        public Vector2 playerCenter;
        public int immunityTimer = 0;

        private Rectangle animRect;
        private Texture2D currentSheet;
        private int frame = 0;
        private int frameCounter = 0;
        private Direction direction;
        private bool walking = false;
        private int shootTimer = 0;
        private Vector2 gunPosition;
        private float gunRotation;
        private int collisionStuckTimer = 0;

        public static GunType gunType = GunType.Rifle;

        public enum Direction
        {
            Front,
            Right,
            Back,
            Left
        }

        public enum GunType
        {
            None,
            Pistol,
            Shotgun,
            Rifle,
            Minigun,
        }


        public override void Initialize()
        {
            hitbox.Width = PlayerWidth;
            hitbox.Height = PlayerHeight;
        }

        public override void Update()
        {
            if (Main.playerHealth <= 0)
            {
                Main.FadeOut();
                if (Main.fadeProgress >= 1f)
                {
                    Main.gameState = Main.GameState.GameOver;
                }
                return;
            }

            if (shootTimer > 0)
                shootTimer--;
            if (immunityTimer > 0)
                immunityTimer--;
            DetectCollisions(Main.entitiesList);
            KeyboardState keyboardState = Keyboard.GetState();


            walking = false;
            Vector2 velocity = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W) && !DetectTileCollisions(position + new Vector2(PlayerWidth / 2f, 16f)))
            {
                velocity.Y -= moveSpeed;
                direction = Direction.Back;
            }
            if (keyboardState.IsKeyDown(Keys.A) && !DetectTileCollisions(position + new Vector2(0, 18f)))
            {
                velocity.X -= moveSpeed;
                direction = Direction.Left;
            }
            if (keyboardState.IsKeyDown(Keys.S) && !DetectTileCollisions(position + new Vector2(PlayerWidth / 2f, PlayerHeight)))
            {
                velocity.Y += moveSpeed;
                direction = Direction.Front;
            }
            if (keyboardState.IsKeyDown(Keys.D) && !DetectTileCollisions(position + new Vector2(PlayerWidth, 18f)))
            {
                velocity.X += moveSpeed;
                direction = Direction.Right;
            }
            /*if (keyboardState.IsKeyDown(Keys.R) && shootTimer <= 0)
            {
                shootTimer += 30;
                int roomWidth = Main.random.Next(20, 40 + 1);
                int roomHeight = Main.random.Next(20, 40 + 1);
                Map.activeMap = Map.GenerateRoom(0, roomWidth, roomHeight);
                Map.mapDimensions[0] = new Vector2(roomWidth, roomHeight);
            }*/

            if (velocity != Vector2.Zero)
            {
                //if (!DetectTileCollisions(playerCenter + (velocity * new Vector2(3.3f, 5f))))
                walking = true;
                collisionStuckTimer = 0;
            }
            else
            {
                if (DetectTileCollisions(position + new Vector2(0f, 17f)))
                {
                    collisionStuckTimer++;
                }
                if (collisionStuckTimer >= 3 * 60)
                {
                    if (!DetectTileCollisions(position))
                        position += new Vector2(0f, 0.3f);
                }
            }

            position += velocity;
            playerCenter = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            AnimatePlayer();
            Main.UpdateCamera(position);

            Vector2 directionToMouse = Main.mouseMapPos - position;
            directionToMouse.Normalize();
            UpdateGunPosition(directionToMouse);
            HandleShooting(directionToMouse);
        }

        public void AnimatePlayer()
        {
            frameCounter++;

            int maxFrame = 1;
            if (walking)
            {
                maxFrame = 4;
                currentSheet = playerWalkingSpritesheets[(int)direction];
            }
            else
            {
                currentSheet = playerIdleSpritesheets[(int)direction];
            }

            if (frameCounter >= 12)
            {
                frame++;
                frameCounter = 0;
            }

            if (frame >= maxFrame)
            {
                frame = 0;
            }

            int frameHeight = currentSheet.Height / maxFrame;
            animRect = new Rectangle(0, frame * frameHeight, PlayerWidth, PlayerHeight);
        }

        private void UpdateGunPosition(Vector2 directionToMouse)
        {
            Vector2 gunDirection = directionToMouse * GunRadius;
            gunPosition = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f) + gunDirection;
            gunRotation = (float)Math.Atan2(gunDirection.Y, gunDirection.X);
        }

        private void HandleShooting(Vector2 directionToMouse)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && gunType != GunType.None && shootTimer <= 0)
            {
                Vector2 shootVelocity = directionToMouse;
                switch (gunType)
                {
                    case GunType.Pistol:
                        shootTimer += 45;
                        shootVelocity *= 3.5f;
                        Bullet.NewBullet(gunPosition, shootVelocity);
                        break;
                    case GunType.Shotgun:
                        shootTimer += 80;

                        float spreadAngle = (float)MathHelper.ToRadians(20f);
                        for (int i = 0; i < 4; i++)
                        {
                            float angleSpread = i * (spreadAngle / 4);
                            float shootAngle = (float)Math.Atan2(shootVelocity.Y + angleSpread, shootVelocity.X + angleSpread);
                            Vector2 velocity = new Vector2((float)Math.Cos(shootAngle), (float)Math.Sin(shootAngle));
                            velocity.Normalize();
                            velocity *= 4f;
                            Bullet.NewBullet(gunPosition, velocity);
                        }
                        break;
                    case GunType.Rifle:
                        shootTimer += 15;
                        shootVelocity *= 4f;
                        Bullet.NewBullet(gunPosition, shootVelocity);
                        break;
                    case GunType.Minigun:
                        shootTimer += 6;
                        shootVelocity *= 5f;
                        Bullet.NewBullet(gunPosition, shootVelocity);
                        break;
                }
                gunShotSounds[(int)gunType - 1].Play();
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (immunityTimer > 0)
                return;

            if (collider is Enemy)
            {
                Main.playerHealth -= 1;
                immunityTimer += 30;
            }
            if (collider is TrashTankBullet)
            {
                Main.playerHealth -= 1;
                immunityTimer += 30;
                collider.DestroyInstance();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (currentSheet == null)
                return;

            Vector2 playerGunOrigin = new Vector2(playerGunTextures[(int)gunType].Width / 2f, playerGunTextures[(int)gunType].Height / 2f);
            SpriteEffects gunEffect = SpriteEffects.None;
            if (gunPosition.X < position.X)
            {
                gunEffect = SpriteEffects.FlipVertically;
            }
            if (gunPosition.Y < position.Y)
            {
                spriteBatch.Draw(playerGunTextures[(int)gunType], gunPosition - Main.cameraPosition, null, Color.White, gunRotation, playerGunOrigin, 1f, gunEffect, 0f);
            }
            spriteBatch.Draw(currentSheet, position - Main.cameraPosition, animRect, Color.White);
            if (gunPosition.Y >= position.Y)
            {
                spriteBatch.Draw(playerGunTextures[(int)gunType], gunPosition - Main.cameraPosition, null, Color.White, gunRotation, playerGunOrigin, 1f, gunEffect, 0f);
            }
        }
    }
}
