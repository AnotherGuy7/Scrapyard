using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Tight_Budget.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D craftBackgroundPanel;
        private Texture2D minimapTexture;
        public static Texture2D scrapTexture;
        private int minimapUpdateTimer = 0;
        private int craftUIShowTimer = 0;
        private int craftPressTimer = 0;
        private bool craftUIShowing = false;

        private const int AmountOfWeapons = 4;
        public static Texture2D[] craftButtonIcons = new Texture2D[AmountOfWeapons];
        private Button[] craftButtons;
        private int[] craftButtonCosts = new int[AmountOfWeapons] { 5, 10, 20, 30 };
        private Vector2[] craftButtonPositions = new Vector2[AmountOfWeapons] { new Vector2(30f, 30f), new Vector2(80f, 30f), new Vector2(30f, 80f), new Vector2(80f, 80f) };

        private Vector2 backgroundPanelPosition;

        public static Texture2D rulesbackgroundPanel;
        private bool rulesShowing = false;

        public static void NewPlayerUI(bool showingRules)
        {
            PlayerUI playerUI = new PlayerUI();
            playerUI.craftButtons = new Button[AmountOfWeapons];
            playerUI.backgroundPanelPosition = new Vector2(Main.halfScreenWidth, Main.halfScreenHeight) - new Vector2(craftBackgroundPanel.Width / 2f, craftBackgroundPanel.Height / 2f);
            for (int i = 0; i < AmountOfWeapons; i++)
            {
                playerUI.craftButtons[i] = new Button(craftButtonIcons[i], 25, 25, playerUI.backgroundPanelPosition + playerUI.craftButtonPositions[i], 0.65f, 0.6f);
            }
            playerUI.rulesShowing = showingRules;
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

            if (Keyboard.GetState().IsKeyDown(Keys.C) && craftUIShowTimer <= 0)
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

                if (craftPressTimer > 0)
                {
                    craftPressTimer--;
                    return;
                }

                for (int buttonIndex = 0; buttonIndex < AmountOfWeapons; buttonIndex++)
                {
                    if (craftButtons[buttonIndex].buttonPressed && Main.playerScrap >= craftButtonCosts[buttonIndex] && Player.gunType != (Player.GunType)buttonIndex + 1)
                    {
                        craftPressTimer += 40;
                        Player.gunType = (Player.GunType)(buttonIndex + 1);
                        Main.playerScrap -= craftButtonCosts[buttonIndex];
                    }
                }
            }

            if (rulesShowing)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && craftUIShowTimer <= 0)     //It just uses the same timer cause why nots
                {
                    craftUIShowTimer += 30;
                    rulesShowing = false;
                    Main.stopUpdates = false;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Main.mainFont, "Health: " + Main.playerHealth, Vector2.Zero, Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Main.mainFont, "Scrap: " + Main.playerScrap, new Vector2(0, 12f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Main.mainFont, "Area: " + (Map.activeMapIndex + 1), new Vector2(0, 24f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

            if (minimapTexture != null)
            {
                Vector2 minimapPosition = new Vector2(Main.halfScreenWidth * 2f, Main.halfScreenHeight * 2f) - new Vector2(minimapTexture.Width + 5f, minimapTexture.Height + 5f);
                spriteBatch.Draw(minimapTexture, minimapPosition, null, Color.White);
            }

            if (craftUIShowing)
            {
                spriteBatch.Draw(craftBackgroundPanel, backgroundPanelPosition, null, Color.White);

                for (int buttonIndex = 0; buttonIndex < AmountOfWeapons; buttonIndex++)
                {
                    Color buttonColor = Color.White;
                    if (craftButtons[buttonIndex].buttonHover)
                    {
                        buttonColor = Color.Yellow;
                    }
                    if (craftButtons[buttonIndex].buttonPressed)
                    {
                         buttonColor = Color.Orange;
                    }
                    if (Main.playerScrap < craftButtonCosts[buttonIndex])
                    {
                        buttonColor = Color.Gray;
                    }
                    if (Player.gunType == (Player.GunType)(buttonIndex + 1))
                    {
                        buttonColor = Color.Red;
                    }
                    craftButtons[buttonIndex].drawColor = buttonColor;
                    craftButtons[buttonIndex].Draw(spriteBatch);

                    Vector2 scrapDrawPosition = craftButtons[buttonIndex].buttonPosition + new Vector2(-4f, craftButtons[buttonIndex].buttonHeight);
                    spriteBatch.Draw(scrapTexture, scrapDrawPosition, buttonColor);

                    spriteBatch.DrawString(Main.mainFont, craftButtonCosts[buttonIndex].ToString(), scrapDrawPosition + new Vector2(scrapTexture.Width + 4f, 0f), buttonColor);
                    spriteBatch.DrawString(Main.mainFont, "Craft A Weapon", backgroundPanelPosition + new Vector2(30f, 2f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                }
            }

            if (rulesShowing)
            {
                Vector2 rulesPanelPosition = new Vector2(Main.halfScreenWidth, Main.halfScreenHeight);
                Vector2 rulesPanelOrigin = new Vector2(rulesbackgroundPanel.Width / 2f, rulesbackgroundPanel.Height / 2f);
                spriteBatch.Draw(rulesbackgroundPanel, rulesPanelPosition, null, Color.White, 0f, rulesPanelOrigin, 1f, SpriteEffects.None, 0f);

                string rules = "Goal: Reach Area #20 safely.\n\n" + 
                    "To craft weapons, press 'C' and click on the weapon that you would like to craft. The appropriate amount of Scrap is needed to make weapons.\n\n" +
                    "To Move, use WASD; To shoot, use Left-click.\n\n" + 
                    "Press SPACE to start playing!";
                string wrappedRules = WrapText(rules, 40);
                spriteBatch.DrawString(Main.mainFont, wrappedRules, rulesPanelPosition - rulesPanelOrigin + new Vector2(5f, 5f), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            }
        }

        private string WrapText(string textToBreak, int sentenceCharacterLimit)
        {
            List<string> createdSentences = new List<string>();
            string[] stringSplitParts = new string[1] { " " };
            string[] wordsArray = textToBreak.Split(stringSplitParts, System.StringSplitOptions.None);

            string sentenceResult = "";
            for (int word = 0; word < wordsArray.Length; word++)
            {
                if (wordsArray[word].Contains("\n"))
                {
                    createdSentences.Add(sentenceResult);
                    sentenceResult = wordsArray[word] + " ";
                    continue;
                }

                if (sentenceResult.Length + wordsArray[word].Length > sentenceCharacterLimit)
                {
                    createdSentences.Add(sentenceResult);
                    sentenceResult = "\n" + wordsArray[word] + " ";
                }
                else
                {
                    sentenceResult += wordsArray[word] + " ";
                }
            }
            if (sentenceResult != "")       //Cause sometimes it doesn't fill the needed conditions to be considered something to add
            {
                createdSentences.Add(sentenceResult);
            }

            string finalResult = "";
            foreach (string sentencePiece in createdSentences)
            {
                finalResult += sentencePiece;
            }
            return finalResult;
        }
    }
}
