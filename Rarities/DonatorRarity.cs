using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class DonatorRarity : ModRarity
	{
		public override Color RarityColor => Color.SpringGreen;

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}
}