using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tight_Budget.UI
{
    public class TextButton : UIObject
    {
        public Texture2D texture;
        public Color drawColor;
        public Vector2 buttonPosition;
        public bool buttonPressed = false;
        public bool buttonHover = false;
        public int buttonWidth = 0;
        public int buttonHeight = 0;
        public string buttonText;

        private float scale;
        private float defaultScale;
        private float hoverScale;
        private Rectangle hitbox;
        private Color idleColor;
        private Color hoverColor;
        private bool drawPanel;

        public TextButton(string text, Vector2 position, float hoverScale, float defaultScale, Color idleColor, Color hoverColor, bool drawPanel = false)
        {
            buttonText = text;
            Vector2 size = Main.mainFont.MeasureString(text) * defaultScale;
            buttonWidth = (int)size.X;
            buttonHeight = (int)size.Y;
            hitbox = new Rectangle((int)position.X, (int)position.Y, buttonWidth, buttonHeight);
            this.defaultScale = defaultScale;
            this.hoverScale = hoverScale;
            buttonPosition = position;
            this.idleColor = idleColor;
            this.hoverColor = hoverColor;
            texture = Main.CreatePanelTexture(buttonWidth + 7, buttonHeight, 1, Color.Black, Color.White);
            this.drawPanel = drawPanel;
        }

        public override void Update()
        {
            scale = defaultScale;
            drawColor = idleColor;
            buttonHover = false;
            buttonPressed = false;
            if (hitbox.Contains(Main.mouseScreenPos.ToPoint()))
            {
                scale = hoverScale;
                buttonHover = true;
                drawColor = hoverColor;
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
            if (drawPanel)
                spriteBatch.Draw(texture, buttonPosition, null, drawColor, 0f, Vector2.Zero, scale + 0.3f, SpriteEffects.None, 0f);

            spriteBatch.DrawString(Main.mainFont, buttonText, buttonPosition, drawColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
