using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class KingdomRarity : ModRarity
	{
		public override Color RarityColor => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(141, 134, 135), new Color(241, 165, 62), new Color(141, 134, 135));

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}
}