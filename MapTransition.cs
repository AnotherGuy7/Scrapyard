using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget
{
    public class MapTransition : CollisionBody
    {
        public override CollisionType collisionType => CollisionType.None;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Player };

        private bool nextMap = false;
        private Vector2 playerSpawnOffset;

        public static MapTransition NewMapTransition(Vector2 position, Vector2 playerSpawnOffset, bool next)
        {
            MapTransition mapTransition = new MapTransition();
            mapTransition.nextMap = next;
            mapTransition.position = position;
            mapTransition.playerSpawnOffset = playerSpawnOffset;
            mapTransition.hitbox = new Rectangle((int)position.X, (int)position.Y, 16, 16);
            return mapTransition;
        }

        public override void Update()
        {
            DetectCollisions(Main.entitiesList);
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            if (nextMap)
            {
                if (Map.activeMapIndex >= 19)
                {
                    Main.stopUpdates = true;
                    Main.gameState = Main.GameState.GameOver;
                    Main.gameWon = true;
                    return;
                }
                Map.LoadNextMap();
                Main.player.position = Map.currentEntrance.position + Map.currentEntrance.playerSpawnOffset;
                //Main.CheckZoomLimits();
                Main.UpdateCamera(Main.player.position);
                Main.player.immunityTimer = 80;
            }
            else
            {
                Map.LoadPreviousMap();
                Main.player.position = Map.currentExit.position + Map.currentExit.playerSpawnOffset;
                //Main.CheckZoomLimits();
                Main.UpdateCamera(Main.player.position);
                Main.player.immunityTimer = 80;
            }
        }
    }
}
