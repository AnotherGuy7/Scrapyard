using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tight_Budget.UI
{
    public class Button : UIObject
    {
        public Texture2D texture;
        public Texture2D iconTexture;
        public Color drawColor;
        public bool buttonPressed = false;

        private float scale;
        private float defaultScale;
        private float hoverScale;
        private Rectangle hitbox;
        private Vector2 buttonPosition;

        public Button(Texture2D iconTexture, int width, int height, Vector2 position, int hoverScale, int defaultScale)
        {
            this.iconTexture = iconTexture;
            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
            this.defaultScale = defaultScale;
            this.hoverScale = hoverScale;
            buttonPosition = position;
            texture = Main.CreatePanelTexture(width, height, 1, Color.Black, Color.White);
        }

        public override void Update()
        {
            scale = defaultScale;
            drawColor = Color.White;
            if (hitbox.Contains(Main.mousePos.ToPoint()))
            {
                scale = hoverScale;
                drawColor = Color.Orange;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    buttonPressed = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, buttonPosition, null, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            float iconScaleX = hitbox.Width - 2 / iconTexture.Width;
            float iconScaleY = hitbox.Height - 2 / iconTexture.Height;
            Vector2 iconPosition = buttonPosition + new Vector2(1f, 1f);
            spriteBatch.Draw(iconTexture, iconPosition, null, drawColor, 0f, Vector2.Zero, new Vector2(iconScaleX, iconScaleY), SpriteEffects.None, 0f);
        }
    }
}
