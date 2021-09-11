using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class TurquoiseRarity : ModRarity
	{
		public override Color RarityColor => new(0, 255, 200);

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			if (offset < 0)
			{
				return ItemRarityID.Purple;
			}

			return Type;
		}
	}
}