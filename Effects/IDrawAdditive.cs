using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption
{
	public interface IDrawAdditive
	{
		void AdditiveCall(SpriteBatch sb, Vector2 screenPos);
	}
}
