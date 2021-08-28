using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Redemption.Globals
{
	public class LegendaryRarity : ModRarity
	{
		public override Color RarityColor => new(255, 195, 0);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}
}