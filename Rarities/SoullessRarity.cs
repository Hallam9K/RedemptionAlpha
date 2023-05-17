using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class SoullessRarity : ModRarity
	{
		public override Color RarityColor => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(59, 61, 87), new Color(110, 115, 157), new Color(59, 61, 87));

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}
}