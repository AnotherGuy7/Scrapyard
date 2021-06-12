using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tight_Budget.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D craftBackgroundPanel;
        private Texture2D minimapTexture;
        private int minimapUpdateTimer = 0;
        private int craftUIShowTimer = 0;
        private int craftPressTimer = 0;
        private bool craftUIShowing = false;

        private Button[] craftButtons;
        private Texture2D[] craftButtonIcons;
        private int[] craftButtonCosts;

        private const int AmountOfWeapons = 2;
        private const int ScrapPistolButton = 0;
        private const int ShotgunButton = 1;

        public static void NewPlayerUI()
        {
            PlayerUI playerUI = new PlayerUI();
            playerUI.craftButtons = new Button[AmountOfWeapons];
            playerUI.craftButtonIcons = new Texture2D[AmountOfWeapons];
            playerUI.craftButtonCosts = new int[AmountOfWeapons];
            Main.uiList.Add(playerUI);
        }

        public override void Update()
        {
            if (minimapUpdateTimer > 0)
                minimapUpdateTimer--;
            if (craftUIShowTimer > 0)
                craftUIShowTimer--;
            if (craftPressTimer > 0)
                craftPressTimer--;


            if (!Main.stopUpdates && Map.activeMap != null && minimapUpdateTimer <= 0)
            {
                minimapUpdateTimer += 5;
                int textureWidth = (int)Map.mapDimensions[Map.activeMapIndex].X;
                int textureHeight = (int)Map.mapDimensions[Map.activeMapIndex].Y;
                minimapTexture = Main.CreateTexture(textureWidth, textureHeight);
                Color[] colorArray = new Color[textureWidth * textureHeight];
                int colorArrayIndex = -1;
                int activeMapFirstDimensionLength = Map.activeMap.GetLength(0);
                for (int x = 0; x < activeMapFirstDimensionLength; x++)
                {
                    for (int y = 0; y < Map.activeMap.GetLength(1); y++)
                    {
                        colorArrayIndex++;
                        Texture2D tileTexture = Map.activeMap[x, y].texture;
                        Color tileColor = Color.White;
                        if (tileTexture == Tile.groundTexture || tileTexture == Tile.groundWithARockTexture || tileTexture == Tile.groundWithAWeirdHoleTexture)
                        {
                            tileColor = new Color(210, 150, 69);
                        }
                        else if (tileTexture == Tile.scrapTexture || tileTexture == Tile.scrapTopTexture)
                        {
                            tileColor = Color.Gray;
                        }
                        else if (tileTexture == Tile.exitDirectionalArrowTexture)
                        {
                            tileColor = Color.Red;
                        }
                        else if (tileTexture == Tile.entranceDirectionalArrowTexture)
                        {
                            tileColor = Color.Blue;
                        }
                        else if (tileTexture == Tile.oldTiresTexture)
                        {
                            tileColor = Color.DarkGray;
                        }
                        else if (tileTexture == Tile.garbagePile)
                        {
                            tileColor = Color.DarkGreen;
                        }
                        colorArray[x + y * activeMapFirstDimensionLength] = tileColor;

                        if ((Main.player.position / 16).ToPoint() == new Point(x, y))
                            colorArray[x + y * activeMapFirstDimensionLength] = Color.LightGreen;
                    }
                }
                minimapTexture.SetData(colorArray);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && craftUIShowTimer <= 0)
            {
                craftUIShowTimer += 30;
                Main.stopUpdates = craftUIShowing = !craftUIShowing;
            }

            if (craftUIShowing)
            {
                foreach (Button button in craftButtons)
                {
                    button.Update();
                }

                if (craftButtons[ScrapPistolButton].buttonPressed)
                {
                    
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Main.mainFont, "Health: " + Main.playerHealth, Vector2.Zero, Color.White);
            spriteBatch.DrawString(Main.mainFont, "Scrap: " + Main.playerScrap, new Vector2(0, 20f), Color.White);

            if (minimapTexture != null)
            {
                Vector2 minimapPosition = new Vector2(270f, 163f) - new Vector2(minimapTexture.Width + 5f, minimapTexture.Height + 5f);
                spriteBatch.Draw(minimapTexture, minimapPosition, null, Color.White);
            }

            if (craftUIShowing)
            {
                spriteBatch.Draw(craftBackgroundPanel, new Vector2((Main._graphics.PreferredBackBufferWidth / 2f) / 3f, (Main._graphics.PreferredBackBufferHeight / 2f) / 3f), null, Color.White);

                foreach (Button button in craftButtons)
                {
                    button.Draw(spriteBatch);
                }
            }
        }
    }
}
