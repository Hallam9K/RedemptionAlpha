using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Waters
{
	public class SoullessWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => Find<ModWaterfallStyle>("Redemption/SoullessWaterfallStyle").Slot;
		public override int GetSplashDust() => Find<ModDust>("Redemption/SoullessWaterSplash").Type; 
		public override int GetDropletGore() => Find<ModGore>("Redemption/SoullessDroplet").Type;
		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
		public override Color BiomeHairColor()
		{
			return Color.Black;
		}
	}
}