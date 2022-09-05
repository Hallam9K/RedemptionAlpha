using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class CosmicRarity : ModRarity
	{
		public override Color RarityColor => RedeColor.NebColour;

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}
}