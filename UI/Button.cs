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
        public Vector2 buttonPosition;
        public bool buttonPressed = false;
        public bool buttonHover = false;
        public int buttonWidth = 0;
        public int buttonHeight = 0;

        private float scale;
        private float defaultScale;
        private float hoverScale;
        private Rectangle hitbox;

        public Button(Texture2D iconTexture, int width, int height, Vector2 position, float hoverScale, float defaultScale)
        {
            this.iconTexture = iconTexture;
            buttonWidth = (int)(width * defaultScale);
            buttonHeight = (int)(height * defaultScale);
            hitbox = new Rectangle((int)position.X - (buttonWidth / 2), (int)position.Y - (buttonHeight / 2), buttonWidth, buttonHeight);
            this.defaultScale = defaultScale;
            this.hoverScale = hoverScale;
            buttonPosition = position;
            texture = Main.CreatePanelTexture(width, height, 1, Color.Black, Color.White);
        }

        public override void Update()
        {
            scale = defaultScale;
            drawColor = Color.White;
            buttonHover = false;
            buttonPressed = false;
            if (hitbox.Contains(Main.mouseScreenPos.ToPoint()))
            {
                scale = hoverScale;
                buttonHover = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    buttonPressed = true;
                }
            }


            hitbox.X = (int)buttonPosition.X;
            hitbox.Y = (int)buttonPosition.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, buttonPosition, null, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            Vector2 iconPosition = buttonPosition + new Vector2(buttonWidth / 2f, buttonHeight / 2f);
            Vector2 iconOrigin = new Vector2(iconTexture.Width / 2f, iconTexture.Height / 2f);
            spriteBatch.Draw(iconTexture, iconPosition, null, drawColor, 0f, iconOrigin, scale, SpriteEffects.None, 0f);
        }
    }
}
