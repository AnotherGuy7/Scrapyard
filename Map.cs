using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Tight_Budget.Enemies;

namespace Tight_Budget
{
    public class Map
    {
        public static Tile[,] activeMap;
        public static int activeMapIndex = 0;
        public static Tile[][,] generatedMapsArray;
        public static List<List<object>> generatedMapObjects = new List<List<object>>();        //A list in a list, weird stuff
        //public static List<object[]> decorationsList = new List<object[]>();
        public static Vector2[] mapDimensions;
        public static MapTransition[] entrances;
        public static MapTransition[] exits;

        public static MapTransition currentEntrance;
        public static MapTransition currentExit;

        public static int[,] structure1 = new int[3, 3]        //I wonder if there's a better way to do this :/
        {
            { 0, 2, 0 },
            { 1, 2, 1 },
            { 0, 1, 0 }
        };

        public static int[,] structure2 = new int[3, 3]
        {
            { 1, 0, 1 },
            { 0, 0, 0 },
            { 1, 0, 1 }
        };

        public static int[,] structure3 = new int[3, 8]
        {
            { 1, 0, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 0, 1 }
        };

        public static int[,] structure4 = new int[6, 10]
        {
            { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1 },
            { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 }
        };

        public static int[,] structure5 = new int[6, 6]
{
            { 1, 1, 0, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
            { 1, 0, 0, 0, 0, 1 },
            { 1, 1, 0, 0, 1, 1 },
};

        public static int[][,] availableStructues = new int[5][,] { structure1, structure2, structure3, structure4, structure5 };

        public static void Update()
        {
            if (currentEntrance != null)
                currentEntrance.Update();

            if (currentExit != null)
                currentExit.Update();
        }

        public static Tile[,] GenerateRoom(int generationIndex, int roomWidth, int roomHeight)       //Wonky..!
        {
            int maxRoomWidth = roomWidth - 1;
            int maxRoomHeight = roomHeight - 1;

            bool exitOnColumn = Main.random.Next(0, 2) == 0;
            int exitPlacement = 1;
            if (exitOnColumn)
            {
                exitPlacement = Main.random.Next(1, maxRoomWidth);
            }
            else
            {
                exitPlacement = Main.random.Next(1, maxRoomHeight);
            }
            int exitDirection = Main.random.Next(0, 2);

            bool entranceOnColumn = false;
            int entrancePlacement = -1;
            int entranceDirection = -1;
            if (generationIndex > 0)
            {
                entranceOnColumn = exits[generationIndex - 1].position.X == 0f || exits[generationIndex - 1].position.X == mapDimensions[generationIndex - 1].X / 16f;
                if (entranceOnColumn)
                    entrancePlacement = Main.random.Next(1, maxRoomWidth);
                else
                    entrancePlacement = Main.random.Next(1, maxRoomHeight);

                if (exits[generationIndex - 1].position.X == 0f || exits[generationIndex - 1].position.Y == 0f)
                    entranceDirection = 1;
                else
                    entranceDirection = 0;
            }

            int row = 0;
            int column = -1;
            Tile[,] map = new Tile[roomWidth, roomHeight];
            for (int i = 0; i < map.Length; i++)
            {
                column++;
                if (column > maxRoomWidth)
                {
                    row += 1;
                    column = 0;
                }

                map[column, row] = new Tile();
                if (row == 0 || row == maxRoomHeight)
                {
                    map[column, row].texture = Tile.scrapTexture;
                    map[column, row].collisionStyle = Tile.CollisionStyle.Solid;
                }
                if (column == 0 || column == maxRoomWidth)
                {
                    map[column, row].texture = Tile.scrapTopTexture;
                    map[column, row].collisionStyle = Tile.CollisionStyle.Solid;
                }
                if (row != 0 && row != maxRoomHeight && column != 0 && column != maxRoomWidth)
                {
                    if (Main.random.Next(0, 25 + 1) != 0)
                        map[column, row].texture = Tile.groundTexture;
                    else
                        if (Main.random.Next(0, 20) != 0)
                            map[column, row].texture = Tile.groundWithARockTexture;
                        else
                            map[column, row].texture = Tile.groundWithAWeirdHoleTexture;
                    map[column, row].collisionStyle = Tile.CollisionStyle.None;
                }
                if (exitOnColumn)
                {
                    if (row == exitDirection * maxRoomHeight)
                    {
                        if (column == exitPlacement)
                        {
                            map[column, row].texture = Tile.exitDirectionalArrowTexture;
                            map[column, row].collisionStyle = Tile.CollisionStyle.None;

                            Vector2 spawnOffset = Vector2.Zero;
                            if (exitDirection == 0)
                            {
                                map[column, row].rotation = 180f;
                                spawnOffset = new Vector2(0f, 32f);
                            }
                            else
                            {
                                map[column, row].rotation = 0f;
                                spawnOffset = new Vector2(0f, -32f);
                            }
                            exits[generationIndex] = MapTransition.NewMapTransition(new Vector2(column * 16f, row * 16f), spawnOffset, true);
                        }
                    }
                }
                else
                {
                    if (column == exitDirection * maxRoomWidth)
                    {
                        if (row == exitPlacement)
                        {
                            map[column, row].texture = Tile.exitDirectionalArrowTexture;
                            map[column, row].collisionStyle = Tile.CollisionStyle.None;

                            Vector2 spawnOffset = Vector2.Zero;
                            if (exitDirection == 0)
                            {
                                map[column, row].rotation = 90f;
                                spawnOffset = new Vector2(32f, 0f);
                            }
                            else
                            {
                                map[column, row].rotation = 270f;
                                spawnOffset = new Vector2(-32f, 0f);
                            }
                            exits[generationIndex] = MapTransition.NewMapTransition(new Vector2(column * 16f, row * 16f), spawnOffset, true);
                        }
                        if (row == exitPlacement - 1)
                        {
                            map[column, row].texture = Tile.scrapTexture;
                        }
                    }
                }

                if (entrancePlacement != -1)
                {
                    if (entranceOnColumn)
                    {
                        if (row == entranceDirection * maxRoomHeight && column == entrancePlacement)
                        {
                            map[column, row].texture = Tile.entranceDirectionalArrowTexture;
                            map[column, row].collisionStyle = Tile.CollisionStyle.None;

                            Vector2 spawnOffset = Vector2.Zero;
                            if (entranceDirection == 0)
                            {
                                map[column, row].rotation = 180f;
                                spawnOffset = new Vector2(0f, 32f);
                            }
                            else
                            {
                                map[column, row].rotation = 0f;
                                spawnOffset = new Vector2(0f, -32f);
                            }
                            entrances[generationIndex] = MapTransition.NewMapTransition(new Vector2(column * 16f, row * 16f), spawnOffset, false);
                        }
                    }
                    else
                    {
                        if (column == entranceDirection * maxRoomWidth)
                        {
                            if (row == entrancePlacement)
                            {
                                map[column, row].texture = Tile.entranceDirectionalArrowTexture;
                                map[column, row].collisionStyle = Tile.CollisionStyle.None;

                                Vector2 spawnOffset = Vector2.Zero;
                                if (entranceDirection == 0)
                                {
                                    map[column, row].rotation = 90f;
                                    spawnOffset = new Vector2(32f, 0f);
                                }
                                else
                                {
                                    map[column, row].rotation = 270f;
                                    spawnOffset = new Vector2(-32f, 0f);
                                }
                                entrances[generationIndex] = MapTransition.NewMapTransition(new Vector2(column * 16f, row * 16f), spawnOffset, false);
                            }
                            if (row == entrancePlacement - 1)
                            {
                                map[column, row].texture = Tile.scrapTexture;
                            }
                        }
                    }
                }
                map[column, row].position = new Vector2(column * map[column, row].texture.Width, row * map[column, row].texture.Height);
            }
            return map;
        }

        public static void GenerateExtraStructures(int generationIndex, int maxRoomWidth, int maxRoomHeight)
        {
            if (generationIndex == 0)
                return;

            int amountOfStructures = Main.random.Next(3, 7 + 1);
            for (int i = 0; i < amountOfStructures; i++)
            {
                int[,] chosenStructure = availableStructues[Main.random.Next(0, availableStructues.Length)];
                int structureWidth = chosenStructure.GetLength(0);
                int structureHeight = chosenStructure.GetLength(1);
                int posX = Main.random.Next(2, maxRoomWidth - 2 - structureWidth + 1);
                int posY = Main.random.Next(2, maxRoomHeight - 2 - structureHeight + 1);
                for (int x = 0; x < structureWidth; x++)
                {
                    for (int y = 0; y < structureHeight; y++)
                    {
                        Texture2D chosenTexture = Tile.groundTexture;
                        Tile.CollisionStyle chosenCollisionStyle = Tile.CollisionStyle.None;
                        switch (chosenStructure[x, y])
                        {
                            case 1:
                                chosenTexture = Tile.oldTiresTexture;
                                chosenCollisionStyle = Tile.CollisionStyle.Solid;
                                break;
                            case 2:
                                chosenTexture = Tile.garbagePile;
                                chosenCollisionStyle = Tile.CollisionStyle.Solid;
                                break;
                        }
                        generatedMapsArray[generationIndex][posX + x, posY + y].texture = chosenTexture;
                        generatedMapsArray[generationIndex][posX + x, posY + y].collisionStyle = chosenCollisionStyle;
                    }
                }
            }
        }

        public static List<object> GenerateExtras(int generationIndex, int maxRoomWidth, int maxRoomHeight)
        {
            int amountOfObjects = Main.random.Next(4, 8 + 1);
            int amountOfEnemies = Main.random.Next(5, 12 + 1); 
            List<object> objectsList = new List<object>();
            for (int i = 0; i < amountOfObjects; i++)
            {
                float posX = Main.random.Next(16, maxRoomWidth * 16);
                float posY = Main.random.Next(16, maxRoomHeight * 16);
                Vector2 pos = new Vector2(posX, posY);
                objectsList.Add(ScrapMetal.NewScrapMetal(pos));
            }
            if (generationIndex != 0)
            {
                for (int i = 0; i < amountOfEnemies; i++)
                {
                    int type = Main.random.Next(0, 1 + 1);
                    float posX = Main.random.Next(1, maxRoomWidth);     //This is in TILE indexes, so we convert that to map coordinates at spawn
                    float posY = Main.random.Next(1, maxRoomHeight);
                    Vector2 pos = new Vector2(posX, posY);
                    Point positionPoint = pos.ToPoint();

                    while (generatedMapsArray[generationIndex][positionPoint.X, positionPoint.Y].collisionStyle == Tile.CollisionStyle.Solid)
                    {
                        posX = Main.random.Next(1, maxRoomWidth);
                        posY = Main.random.Next(1, maxRoomHeight);
                        pos = new Vector2(posX, posY);
                        positionPoint = pos.ToPoint();
                    }

                    if (type == 0)
                        objectsList.Add(FlyingTrashBag.NewFlyingTrashBag(pos * 16f));
                    if (type == 1)
                        objectsList.Add(MechanizedGarbage.NewMechanizedGarbage(pos * 16f));
                }
            }

            return objectsList;
        }

        public static void GenerateWholeMap()
        {
            int amountOfRooms = 7;
            generatedMapsArray = new Tile[amountOfRooms][,];
            mapDimensions = new Vector2[amountOfRooms];
            entrances = new MapTransition[amountOfRooms];
            exits = new MapTransition[amountOfRooms];
            for (int i = 0; i < amountOfRooms; i++)
            {
                int roomWidth = Main.random.Next(20, 30 + 1);
                int roomHeight = Main.random.Next(20, 30 + 1);
                generatedMapsArray[i] = GenerateRoom(i, roomWidth, roomHeight);
                GenerateExtraStructures(i, roomWidth - 1, roomHeight - 1);
                generatedMapObjects.Add(GenerateExtras(i, roomWidth - 1, roomHeight - 1));
                mapDimensions[i] = new Vector2(roomWidth, roomHeight);
            }
        }

        public static void LoadNextMap(int forcedIndex = -1)
        {
            for (int i = 0; i < generatedMapObjects[activeMapIndex].Count; i++)
            {
                if (generatedMapObjects[activeMapIndex][i] is Enemy)
                {
                    Enemy enemy = generatedMapObjects[activeMapIndex][i] as Enemy;
                    Main.entitiesList.Remove(enemy);
                }
            }

            activeMapIndex += 1;

            if (forcedIndex != -1)
                activeMapIndex = forcedIndex;

            activeMap = generatedMapsArray[activeMapIndex];

            if (entrances[activeMapIndex] != null)
                currentEntrance = entrances[activeMapIndex];

            if (exits[activeMapIndex] != null)
                currentExit = exits[activeMapIndex];

            for (int i = 0; i < generatedMapObjects[activeMapIndex].Count; i++)
            {
                if (generatedMapObjects[activeMapIndex][i] is Enemy)
                {
                    Enemy enemy = generatedMapObjects[activeMapIndex][i] as Enemy;
                    Main.entitiesList.Add(enemy);
                }
            }
        }

        public static void LoadPreviousMap()
        {
            for (int i = 0; i < generatedMapObjects[activeMapIndex].Count; i++)
            {
                if (generatedMapObjects[activeMapIndex][i] is Enemy)
                {
                    Enemy enemy = generatedMapObjects[activeMapIndex][i] as Enemy;
                    Main.entitiesList.Remove(enemy);
                }
            }

            activeMapIndex -= 1;

            if (activeMapIndex < 0)
                activeMapIndex = 0;

            activeMap = generatedMapsArray[activeMapIndex];

            if (entrances[activeMapIndex] != null)
                currentEntrance = entrances[activeMapIndex];

            if (exits[activeMapIndex] != null)
                currentExit = exits[activeMapIndex];

            for (int i = 0; i < generatedMapObjects[activeMapIndex].Count; i++)
            {
                if (generatedMapObjects[activeMapIndex][i] is Enemy)
                {
                    Enemy enemy = generatedMapObjects[activeMapIndex][i] as Enemy;
                    Main.entitiesList.Add(enemy);
                }
            }
        }
    }
}
