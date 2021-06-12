using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget.Enemies
{
    public class MechanizedGarbage : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };
        public override int startingHealth => 3;

        public static Texture2D nechanizedGarbageSpritesheet;
        public static SoundEffect nechanizedGarbageHitSound;

        public const float MoveSpeed = 0.5f;
        public const int GarbageWidth = 17;
        public const int GarbageHeight = 11;

        private int health = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;
        private Color drawColor;
        private int colorChangeTimer = 0;

        public static MechanizedGarbage NewMechanizedGarbage(Vector2 position)
        {
            MechanizedGarbage nechanizedGarbage = new MechanizedGarbage();
            nechanizedGarbage.health = nechanizedGarbage.startingHealth;
            nechanizedGarbage.position = position;
            nechanizedGarbage.hitbox = new Rectangle((int)position.X, (int)position.Y, GarbageWidth, GarbageHeight);
            return nechanizedGarbage;
        }

        public override void Update()
        {
            DetectCollisions(Main.entitiesList);
            Vector2 velocity = Main.player.position - position;
            velocity.Normalize();
            velocity *= MoveSpeed;

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
            center = position + new Vector2(GarbageWidth / 2f, GarbageHeight / 2f);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            if (velocity != Vector2.Zero)
            {
                frameCounter++;
                if (frameCounter >= 8)
                {
                    frame++;
                    frameCounter = 0;
                    if (frame >= 4)
                    {
                        frame = 0;
                    }
                    animRect = new Rectangle(0, GarbageHeight * frame, GarbageWidth, GarbageHeight);
                }
            }
            else
            {
                frame = 0;
                frameCounter = 0;
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
                Map.generatedMapObjects[Map.activeMapIndex].Add(Gore.NewGore(Gore.GarbageGore, center));
            }
            nechanizedGarbageHitSound.Play();
            collider.DestroyInstance();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(nechanizedGarbageSpritesheet, position - Main.cameraPosition, animRect, drawColor);
        }
    }
}
