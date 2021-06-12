using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public static Vector2 mousePos;

        public static int halfScreenWidth = 0;
        public static int halfScreenHeight = 0;
        public static int actualResolutionWidth = 800;
        public static int actualResolutionHeight = 600;
        public static int desiredResolutionWidth = 267;
        public static int desiredResolutionHeight = 200;

        private Matrix screenMatrix;

        public static Player player;
        public static SpriteFont mainFont;
        public static Random random;

        public static int playerHealth = 4;
        public static int playerScrap = 0;

        public static bool stopUpdates = false;

        public static string debugValue = "";


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
            _graphics.PreferredBackBufferWidth = actualResolutionWidth;
            _graphics.PreferredBackBufferHeight = actualResolutionHeight;

            float matrixX = actualResolutionWidth / desiredResolutionWidth;
            float matrixY = actualResolutionHeight / desiredResolutionHeight;

            halfScreenWidth = desiredResolutionWidth / 2;
            halfScreenHeight = desiredResolutionHeight / 2;

            screenMatrix = Matrix.CreateScale(3f, 3f, 1f);

            player = new Player();
            entitiesList.Add(player);
            player.Initialize();
            PlayerUI.NewPlayerUI();
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
            Player.playerGunTexture = LoadTex("ScrapPistol");
            Player.gunShotSound = LoadSFX("Gunshot");
            PlayerUI.craftBackgroundPanel = CreatePanelTexture(80, 60, 2, Color.Black, Color.White);

            Bullet.bulletTexture = LoadTex("Bullet");

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
            MechanizedGarbage.nechanizedGarbageHitSound = LoadSFX("Garbage_Hit");

            ScrapMetal.scrapMetalTexture = LoadTex("ScrapMetal");

            int amountOfGoreTextures = 2;
            Gore.goreTextues = new Texture2D[amountOfGoreTextures];
            for (int i = 0; i < amountOfGoreTextures; i++)
            {
                Gore.goreTextues[i] = LoadTex("Gore/Gore_" + i);
            }

            //Map.GenerateMap(Map.scrapyardMap, Map.TrashMapWidth, Map.TrashMapHeight);
            Map.GenerateWholeMap();
            Map.LoadNextMap(0);
            player.position = (Map.mapDimensions[Map.activeMapIndex] * 16f) / 2f;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //debugValue = "";
            mousePos = cameraPosition + Mouse.GetState().Position.ToVector2() / 3f;

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, screenMatrix);

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

            if (debugValue != "")
            {
                Vector2 position = new Vector2(halfScreenWidth * 2f, halfScreenHeight * 2f) - (mainFont.MeasureString(debugValue) * 0.4f);
                _spriteBatch.DrawString(mainFont, debugValue, Vector2.Zero, Color.Red, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
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
            if (cameraPosition.Y + (halfScreenHeight * 2f) - 36f >= Map.mapDimensions[Map.activeMapIndex].Y * 16f)      //The -36f... I have no idea why the camera is offset by that much, cause the math checks out all the time... :shrug:
            {
                cameraPosition.Y = Map.mapDimensions[Map.activeMapIndex].Y * 16f - (halfScreenHeight * 2f) + 36F;
            }
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
                Color chosenColor = backgroundColor;

                if (x <= outlineSize || x >= width - outlineSize)
                    chosenColor = outlineColor;

                for (int y = 0; y < height; y++)
                {
                    if (y <= outlineSize || y >= height - outlineSize)
                        chosenColor = outlineColor;

                    colorArray[x + y * width] = chosenColor;
                }
            }
            texture.SetData(colorArray);
            return texture;
        }
    }
}
