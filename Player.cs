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
        public static Texture2D playerGunTexture;
        public static SoundEffect gunShotSound;

        public const float moveSpeed = 1.3f;
        public const int PlayerWidth = 13;
        public const int PlayerHeight = 25;
        public const float GunRadius = 18f;

        private Vector2 playerCenter;
        private Rectangle animRect;
        private Texture2D currentSheet;
        private int frame = 0;
        private int frameCounter = 0;
        private Direction direction;
        private bool walking = false;
        private int shootTimer = 0;
        private Vector2 gunPosition;
        private float gunRotation;
        private int immunityTimer = 0;

        public enum Direction
        {
            Front,
            Right,
            Back,
            Left
        }

        public override void Initialize()
        {
            hitbox.Width = PlayerWidth;
            hitbox.Height = PlayerHeight;
        }

        public override void Update()
        {
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
            }

            Main.debugValue = Map.activeMapIndex.ToString();

            position += velocity;
            playerCenter = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            AnimatePlayer();
            Main.UpdateCamera(position);
            UpdateGunPosition();
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

        private void UpdateGunPosition()
        {
            Vector2 direction = Main.mousePos - position;
            direction.Normalize();
            Vector2 gunDirection = direction * GunRadius;
            gunPosition = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f) + gunDirection;
            gunRotation = (float)Math.Atan2(gunDirection.Y, gunDirection.X);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && shootTimer <= 0)
            {
                shootTimer += 30;
                Vector2 shootVelocity = direction * 3.5f;
                Bullet.NewBullet(gunPosition, shootVelocity);
                gunShotSound.Play();
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (immunityTimer <= 0 && collider is Enemy)
            {
                Main.playerHealth -= 1;
                immunityTimer += 30;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 playerGunOrigin = new Vector2(playerGunTexture.Width / 2f, playerGunTexture.Height / 2f);
            if (gunPosition.Y > position.Y)
            {
                spriteBatch.Draw(playerGunTexture, gunPosition - Main.cameraPosition, null, Color.White, gunRotation, playerGunOrigin, 1f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(currentSheet, position - Main.cameraPosition, animRect, Color.White);
            if (gunPosition.Y <= position.Y)
            {
                spriteBatch.Draw(playerGunTexture, gunPosition - Main.cameraPosition, null, Color.White, gunRotation, playerGunOrigin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
