using Microsoft.Xna.Framework;

namespace Tight_Budget.Enemies
{
    public abstract class Enemy : CollisionBody
    {
        public virtual int startingHealth { get; }

        public Vector2 center;

        public new void DestroyInstance()
        {
            Main.amountOfEnemiesKilled += 1;
            Main.entitiesList.Remove(this);
        }
    }
}
