using Microsoft.Xna.Framework.Graphics;

namespace Tight_Budget.UI
{
    public abstract class UIObject
    {
        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }
    }
}
