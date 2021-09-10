using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria.ModLoader;

namespace Redemption.Rarities
{
	public class SoullessRarity : ModRarity
	{
		public override Color RarityColor => BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.DarkGray, Color.Black, Color.DarkGray);
	}
}