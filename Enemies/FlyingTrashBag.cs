using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget.Enemies
{
    public class FlyingTrashBag : Enemy
    {
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.FriendlyProjectiles };
        public override int startingHealth => 2;

        public static Texture2D flyingTrashBagSpritesheet;
        public static SoundEffect flyingTrashBagHitSound;

        public const float MoveSpeed = 0.3f;
        public const int TrashBagWidth = 13;
        public const int TrashBagHeight = 20;

        private int health = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;
        private Color drawColor;
        private int colorChangeTimer = 0;

        public static FlyingTrashBag NewFlyingTrashBag(Vector2 position)
        {
            FlyingTrashBag flyingTrashBag = new FlyingTrashBag();
            flyingTrashBag.health = flyingTrashBag.startingHealth;
            flyingTrashBag.position = position;
            flyingTrashBag.hitbox = new Rectangle((int)position.X, (int)position.Y, TrashBagWidth, TrashBagHeight);
            return flyingTrashBag;
        }

        public override void Update()
        {
            DetectCollisions(Main.entitiesList);
            Vector2 velocity = Main.player.position - position;
            velocity.Normalize();
            velocity *= MoveSpeed;

            position += velocity;
            center = position + new Vector2(TrashBagWidth / 2f, TrashBagHeight / 2f);
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            frameCounter++;
            if (frameCounter >= 3)
            {
                frame++;
                frameCounter = 0;
                if (frame >= 4)
                {
                    frame = 0;
                }
                animRect = new Rectangle(0, TrashBagHeight * frame, TrashBagWidth, TrashBagHeight);
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
                Map.generatedMapObjects[Map.activeMapIndex].Add(Gore.NewGore(Gore.TrashGore, center));
            }
            flyingTrashBagHitSound.Play();
            collider.DestroyInstance();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(flyingTrashBagSpritesheet, position - Main.cameraPosition, animRect, drawColor);
        }
    }
}
