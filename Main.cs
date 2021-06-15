using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using Tight_Budget.Enemies;
using Tight_Budget.Projectiles;
using Tight_Budget.UI;

namespace Tight_Budget
{
    public class Main : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static GameState gameState = GameState.Title;

        public static List<CollisionBody> entitiesList = new List<CollisionBody>();
        public static List<UIObject> uiList = new List<UIObject>();
        public static Vector2 cameraPosition;
        public static Vector2 mouseMapPos;
        public static Vector2 mouseScreenPos;

        public static int halfScreenWidth = 0;
        public static int halfScreenHeight = 0;
        public static int actualResolutionWidth = 800;
        public static int actualResolutionHeight = 600;
        public static int desiredResolutionWidth = 267;
        public static int desiredResolutionHeight = 200;

        private Matrix screenMatrix;
        private Vector2 mouseScreenDivision;

        public static Player player;
        public static SpriteFont mainFont;
        public static Random random;

        public static int playerHealth = 4;
        public static int playerScrap = 0;
        public static int collectedScrap = 0;
        public static int amountOfEnemiesKilled = 0;

        public static bool stopUpdates = false;
        public static bool gameWon = false;
        public static bool fadingOut = false;
        public static bool fadingIn = false;
        public static float fadeProgress = 0f;

        public static string debugValue = "";

        public static Song[] mainMusic;
        public static Song selectedSong;
        public static bool firstGameOn = false;
        private Texture2D titleScreen;
        private Texture2D fadeOutTexture;

        private const int StartButton = 0;
        private const int QuitButton = 1;
        private TextButton[] titleButtons;

        private const int RetryButton = 0;
        private TextButton[] gameOverButtons;
        private Texture2D gameOverPanel;
        private Vector2 gameOverPanelPosition;


        public enum GameState
        {
            Title,
            Playing,
            GameOver
        }

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = actualResolutionWidth;
            _graphics.PreferredBackBufferHeight = actualResolutionHeight;
            _graphics.ApplyChanges();
            Window.AllowUserResizing = true;

            float matrixX = actualResolutionWidth / (float)desiredResolutionWidth;
            float matrixY = actualResolutionHeight / (float)desiredResolutionHeight;
            Window.ClientSizeChanged += OnChangeResolution;

            halfScreenWidth = desiredResolutionWidth / 2;
            halfScreenHeight = desiredResolutionHeight / 2;

            screenMatrix = Matrix.CreateScale(matrixX, matrixY, 1f);
            mouseScreenDivision = new Vector2(matrixX, matrixY);

            player = new Player();
            entitiesList.Add(player);
            player.Initialize();
            LoadContent();

            titleButtons = new TextButton[2];
            titleButtons[StartButton] = new TextButton("Start", new Vector2(3f, 26f), 0.65f, 0.6f, Color.Gray, Color.Green);
            titleButtons[QuitButton] = new TextButton("Quit", new Vector2(3f, 46f), 0.65f, 0.6f, Color.Gray, Color.Red);

            gameOverPanelPosition = new Vector2(halfScreenWidth, halfScreenHeight) - new Vector2(gameOverPanel.Width / 2f, gameOverPanel.Height / 2f);
            gameOverButtons = new TextButton[2];
            gameOverButtons[RetryButton] = new TextButton("Retry", gameOverPanelPosition + new Vector2((gameOverPanel.Width / 2f) - 20f, gameOverPanel.Height - 10f), 0.65f, 0.6f, Color.White, Color.Green, true);
            gameOverButtons[QuitButton] = new TextButton("Quit", gameOverPanelPosition + new Vector2((gameOverPanel.Width / 2f) + 20f, gameOverPanel.Height - 10f), 0.65f, 0.6f, Color.White, Color.Red, true);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            random = new Random();
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");

            Player.playerWalkingSpritesheets = new Texture2D[4];
            Player.playerWalkingSpritesheets[0] = LoadTex("Spritesheets/Walking_Front");
            Player.playerWalkingSpritesheets[1] = LoadTex("Spritesheets/Walking_Right");
            Player.playerWalkingSpritesheets[2] = LoadTex("Spritesheets/Walking_Back");
            Player.playerWalkingSpritesheets[3] = LoadTex("Spritesheets/Walking_Left");

            Player.playerIdleSpritesheets = new Texture2D[4];
            Player.playerIdleSpritesheets[0] = LoadTex("Spritesheets/Idle_Front");
            Player.playerIdleSpritesheets[1] = LoadTex("Spritesheets/Idle_Right");
            Player.playerIdleSpritesheets[2] = LoadTex("Spritesheets/Idle_Back");
            Player.playerIdleSpritesheets[3] = LoadTex("Spritesheets/Idle_Left");
            Player.playerGunTextures = new Texture2D[5];
            Player.playerGunTextures[(int)Player.GunType.None] = LoadTex("ScrapAir");
            Player.playerGunTextures[(int)Player.GunType.Pistol] = LoadTex("ScrapPistol");
            Player.playerGunTextures[(int)Player.GunType.Shotgun] = LoadTex("ScrapShotgun");
            Player.playerGunTextures[(int)Player.GunType.Rifle] = LoadTex("ScrapRifle");
            Player.playerGunTextures[(int)Player.GunType.Minigun] = LoadTex("ScrapMinigun");

            Player.gunShotSounds = new SoundEffect[4];
            Player.gunShotSounds[(int)Player.GunType.Pistol - 1] = LoadSFX("PistolSound");
            Player.gunShotSounds[(int)Player.GunType.Shotgun - 1] = LoadSFX("ShotgunSound");
            Player.gunShotSounds[(int)Player.GunType.Rifle - 1] = LoadSFX("RifleSound");
            Player.gunShotSounds[(int)Player.GunType.Minigun - 1] = LoadSFX("MinigunSound");


            PlayerUI.craftBackgroundPanel = CreatePanelTexture(140, 120, 1, Color.Black, Color.White);
            PlayerUI.rulesbackgroundPanel = CreatePanelTexture(160, 140, 1, Color.Black, Color.White);
            Texture2D[] playerGunTexturesClone = new Texture2D[Player.playerGunTextures.Length - 1];
            for (int i = 1; i < Player.playerGunTextures.Length; i++)
            {
                playerGunTexturesClone[i - 1] = Player.playerGunTextures[i];
            }
            PlayerUI.craftButtonIcons = playerGunTexturesClone;
            PlayerUI.scrapTexture = LoadTex("ScrapMetal");

            Bullet.bulletTexture = LoadTex("Bullet");
            TrashTankBullet.trashTankBulletTexture = LoadTex("TrashTankBullet");

            /*Tile.grassTexture = LoadTex("Tiles/GrassTile");
            Tile.grassyGrassTexture = LoadTex("Tiles/GrassyTile");
            Tile.concreteTexture = LoadTex("Tiles/ConcreteTile");*/
            Tile.groundTexture = LoadTex("Tiles/Ground");
            Tile.groundWithARockTexture = LoadTex("Tiles/GroundWithARock");
            Tile.groundWithAWeirdHoleTexture = LoadTex("Tiles/GroundWithAWeirdHole");
            Tile.scrapTexture = LoadTex("Tiles/Scrap");
            Tile.scrapTopTexture = LoadTex("Tiles/ScrapTop");
            Tile.entranceDirectionalArrowTexture = LoadTex("Tiles/EntranceArrow");
            Tile.exitDirectionalArrowTexture = LoadTex("Tiles/ExitArrow");
            Tile.oldTiresTexture = LoadTex("Tiles/OldTiresTile");
            Tile.garbagePile = LoadTex("Tiles/GarbagePile");

            FlyingTrashBag.flyingTrashBagSpritesheet = LoadTex("Spritesheets/FlyingTrashBag");
            FlyingTrashBag.flyingTrashBagHitSound = LoadSFX("TrashBag_Hit");
            MechanizedGarbage.nechanizedGarbageSpritesheet = LoadTex("Spritesheets/MechanizedGarbage");
            TrashTank.trashTankHitSound = MechanizedGarbage.nechanizedGarbageHitSound = LoadSFX("Garbage_Hit");
            TrashTank.trashTankSpritesheet = LoadTex("Spritesheets/TrashTank");
            TrashTank.trashTankBarrelTexture = LoadTex("TrashTankBarrel");
            TrashTank.trashTankShootSound = LoadSFX("TrashTank_Shoot");

            ScrapMetal.scrapMetalTexture = LoadTex("ScrapMetal");

            int amountOfGoreTextures = 3;
            Gore.goreTextues = new Texture2D[amountOfGoreTextures];
            for (int i = 0; i < amountOfGoreTextures; i++)
            {
                Gore.goreTextues[i] = LoadTex("Gore/Gore_" + i);
            }

            mainMusic = new Song[2];
            mainMusic[0] = Content.Load<Song>("Sounds/Music/Scrapyard_Main");
            mainMusic[1] = Content.Load<Song>("Sounds/Music/Scrapyard_Main_2");
            selectedSong = mainMusic[random.Next(0, mainMusic.Length)];

            titleScreen = LoadTex("TitleScreen");
            gameOverPanel = CreatePanelTexture(180, 120, 1, Color.Black, Color.White);

            Texture2D fadeTexture = CreateTexture(1, 1);
            Color[] color = new Color[1];
            color[0] = Color.Black;
            fadeTexture.SetData(color);
            fadeOutTexture = fadeTexture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            mouseScreenPos = Mouse.GetState().Position.ToVector2() / mouseScreenDivision;
            mouseMapPos = mouseScreenPos + cameraPosition;

            if (gameState == GameState.Title)
            {
                for (int i = 0; i < titleButtons.Length; i++)
                {
                    titleButtons[i].Update();
                }

                if (titleButtons[StartButton].buttonPressed)
                {
                    FadeIn();
                }
                if (titleButtons[QuitButton].buttonPressed)
                {
                    Exit();
                }

                if (fadeProgress >= 1f)
                {
                    InitializeGame();
                    FadeOut();
                }
            }
            else if (gameState == GameState.Playing)
            {
                if (!stopUpdates)
                {
                    object[] mapObjectsCopy = Map.generatedMapObjects[Map.activeMapIndex].ToArray();
                    CollisionBody[] entitiesListCopy = entitiesList.ToArray();
                    foreach (object mapObject in mapObjectsCopy)
                    {
                        if (mapObject is ScrapMetal)
                        {
                            ScrapMetal scrapMetal = mapObject as ScrapMetal;
                            scrapMetal.Update();
                        }
                    }
                    foreach (CollisionBody entity in entitiesListCopy)
                    {
                        entity.Update();
                    }
                }
                UIObject[] uiListCopy = uiList.ToArray();
                foreach (UIObject ui in uiListCopy)
                {
                    ui.Update();
                }

                Map.Update();
            }
            else if (gameState == GameState.GameOver)
            {
                for (int i = 0; i < gameOverButtons.Length; i++)
                {
                    gameOverButtons[i].Update();
                }

                if (gameOverButtons[RetryButton].buttonPressed)
                {
                    FadeIn();
                }

                if (gameOverButtons[QuitButton].buttonPressed)
                {
                    Exit();
                }

                if (fadingIn && fadeProgress >= 1f)
                {
                    InitializeGame();
                    FadeOut();
                }
            }

            if (fadingIn && fadeProgress < 1f)
            {
                fadeProgress += 0.02f;
            }
            if (fadingOut && fadeProgress > 0f)
            {
                fadeProgress -= 0.02f;
            }
            if (selectedSong != null && MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(selectedSong);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, screenMatrix);

            if (gameState == GameState.Title)
            {
                _spriteBatch.Draw(titleScreen, Vector2.Zero, Color.White);

                _spriteBatch.DrawString(mainFont, "Scrapyard", new Vector2(halfScreenWidth, 24f), Color.White, 0f, mainFont.MeasureString("Scrapyard") / 2f, 1f, SpriteEffects.None, 0f);

                foreach (TextButton button in titleButtons)
                {
                    button.Draw(_spriteBatch);
                }
            }
            else if (gameState == GameState.Playing)
            {
                foreach (Tile tile in Map.activeMap)
                {
                    tile.Draw(_spriteBatch);
                }
                CollisionBody[] entitiesListCopy = entitiesList.ToArray();
                UIObject[] uiListCopy = uiList.ToArray();
                object[] mapObjectsCopy = Map.generatedMapObjects[Map.activeMapIndex].ToArray();
                foreach (object mapObject in mapObjectsCopy)
                {
                    if (mapObject is ScrapMetal)
                    {
                        ScrapMetal scrapMetal = mapObject as ScrapMetal;
                        scrapMetal.Draw(_spriteBatch);
                    }
                    if (mapObject is Gore)
                    {
                        Gore gore = mapObject as Gore;
                        gore.Draw(_spriteBatch);
                    }
                }
                foreach (CollisionBody entity in entitiesListCopy)
                {
                    entity.Draw(_spriteBatch);
                }
                foreach (UIObject ui in uiListCopy)
                {
                    ui.Draw(_spriteBatch);
                }
            }
            else if (gameState == GameState.GameOver)
            {
                foreach (Tile tile in Map.generatedMapsArray[0])        //The back of the map
                {
                    tile.Draw(_spriteBatch);
                }
                player.position = (Map.mapDimensions[0] * 16f) / 2f;
                UpdateCamera(player.position);

                _spriteBatch.Draw(gameOverPanel, gameOverPanelPosition, Color.White);

                string gameOverPanelHeader = "You died.";
                if (gameWon)
                    gameOverPanelHeader = "You survived!";

                Vector2 headerOrigin = mainFont.MeasureString(gameOverPanelHeader) / 2f;
                _spriteBatch.DrawString(mainFont, gameOverPanelHeader, gameOverPanelPosition + new Vector2(gameOverPanel.Width / 2f, headerOrigin.Y + 2f), Color.White, 0f, headerOrigin, 1f, SpriteEffects.None, 0f);

                _spriteBatch.DrawString(mainFont, "Leftover Health: " + playerHealth, gameOverPanelPosition + new Vector2(3f, 30f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(mainFont, "Collected Scrap: " + collectedScrap, gameOverPanelPosition + new Vector2(3f, 42f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(mainFont, "Amount of Enemies Killed: " + amountOfEnemiesKilled, gameOverPanelPosition + new Vector2(3f, 54f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(mainFont, "Area Reached: " + (Map.activeMapIndex + 1), gameOverPanelPosition + new Vector2(3f, 66f), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);

                foreach (TextButton button in gameOverButtons)
                {
                    button.Draw(_spriteBatch);
                }
            }

            if (fadeProgress > 0f)
            {
                _spriteBatch.Draw(fadeOutTexture, new Rectangle(0, 0, desiredResolutionWidth, desiredResolutionWidth), Color.White * fadeProgress);
            }

            if (debugValue != "")
            {
                Vector2 debugValuePosition = new Vector2(halfScreenWidth * 2f, halfScreenHeight * 2f) - (mainFont.MeasureString(debugValue) * 0.4f);
                _spriteBatch.DrawString(mainFont, debugValue, debugValuePosition, Color.Red, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            }

            _spriteBatch.End();
        }

        public static void UpdateCamera(Vector2 position)
        {
            cameraPosition = position - new Vector2(halfScreenWidth - (Player.PlayerWidth / 2f), halfScreenHeight - (Player.PlayerHeight / 2f));

            if (cameraPosition.X <= 0)
            {
                cameraPosition.X = 0;
            }
            if (cameraPosition.X + (halfScreenWidth * 2f) - 1 >= Map.mapDimensions[Map.activeMapIndex].X * 16f)
            {
                cameraPosition.X = Map.mapDimensions[Map.activeMapIndex].X * 16f - (halfScreenWidth * 2f) - 1;
            }
            if (cameraPosition.Y <= 0)
            {
                cameraPosition.Y = 0;
            }
            if (cameraPosition.Y + (halfScreenHeight * 2f) + 5f >= Map.mapDimensions[Map.activeMapIndex].Y * 16f)      //The -6f is so that only the top part of those tiles show up
            {
                cameraPosition.Y = Map.mapDimensions[Map.activeMapIndex].Y * 16f - (halfScreenHeight * 2f) - 5f;
            }
        }

        /*public static void ChangeZoom(int zoomAdd)
        {
            if (desiredResolutionWidth + zoomAdd > Map.mapDimensions[Map.activeMapIndex].X * 16f)
                return;

            if (desiredResolutionHeight + zoomAdd > Map.mapDimensions[Map.activeMapIndex].Y * 16f)
                return;

            desiredResolutionWidth += zoomAdd;
            desiredResolutionHeight += zoomAdd;
        }

        public static void CheckZoomLimits()
        {
            if (desiredResolutionWidth < Map.mapDimensions[Map.activeMapIndex].X * 16f)
                desiredResolutionWidth = (int)(Map.mapDimensions[Map.activeMapIndex].X * 16f);

            if (desiredResolutionHeight < Map.mapDimensions[Map.activeMapIndex].Y * 16f)
                desiredResolutionHeight = (int)(Map.mapDimensions[Map.activeMapIndex].Y * 16f);
        }*/

        private void OnChangeResolution(object sender, EventArgs args)
        {
            actualResolutionWidth = Window.ClientBounds.Width;
            actualResolutionHeight = Window.ClientBounds.Height;

            float matrixX = actualResolutionWidth / (float)desiredResolutionWidth;
            float matrixY = actualResolutionHeight / (float)desiredResolutionHeight;

            halfScreenWidth = desiredResolutionWidth / 2;
            halfScreenHeight = desiredResolutionHeight / 2;

            screenMatrix = Matrix.CreateScale(matrixX, matrixY, 1f);
            mouseScreenDivision = new Vector2(matrixX, matrixY);
        }

        public static void InitializeGame()
        {
            entitiesList.Clear();
            uiList.Clear();

            if (!firstGameOn)
            {
                firstGameOn = true;
                PlayerUI.NewPlayerUI(true);
                stopUpdates = true;
                Player.gunType = Player.GunType.None;
            }
            else
            {
                PlayerUI.NewPlayerUI(false);
                Player.gunType = Player.GunType.Pistol;
            }
            gameState = GameState.Playing;
            playerHealth = 4;
            playerScrap = 0;
            collectedScrap = 0;
            amountOfEnemiesKilled = 0;
            gameWon = false;

            player = new Player();
            entitiesList.Add(player);
            player.Initialize();

            Map.GenerateWholeMap();
            Map.LoadNextMap(0);
            player.position = (Map.mapDimensions[Map.activeMapIndex] * 16f) / 2f;
            UpdateCamera(player.position);

            selectedSong = mainMusic[random.Next(0, mainMusic.Length)];
            MediaPlayer.Play(selectedSong);
        }

        public static void FadeIn()
        {
            fadingIn = true;
            fadingOut = false;
            fadeProgress = 0f;
        }

        public static void FadeOut()
        {
            fadingOut = true;
            fadingIn = false;
            fadeProgress = 1f;
        }

        /// <summary>
        /// Shorthand for Content.Load<Texture2D>(Path);
        /// </summary>
        private Texture2D LoadTex(string path)
        {
            return Content.Load<Texture2D>("Textures/" + path);
        }

        private SoundEffect LoadSFX(string path)
        {
            return Content.Load<SoundEffect>("Sounds/" + path);
        }

        public static Texture2D CreateTexture(int width, int height)
        {
            return new Texture2D(_graphics.GraphicsDevice, width, height);
        }

        public static Texture2D CreatePanelTexture(int width, int height, int outlineSize, Color backgroundColor, Color outlineColor)
        {
            Texture2D texture = CreateTexture(width, height);
            Color[] colorArray = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color chosenColor = backgroundColor;

                    if (y < outlineSize || y > height - outlineSize - 1 || x < outlineSize || x > width - outlineSize - 1)
                        chosenColor = outlineColor;

                    if ((x == 0 && y == 0) || (x == 0 && y == height - 1) || (x == width - 1 && y == 0) || (x == width - 1 && y == height - 1))
                        chosenColor = new Color(0, 0, 0, 0);

                    colorArray[x + y * width] = chosenColor;
                }
            }
            texture.SetData(colorArray);
            return texture;
        }
    }
}
