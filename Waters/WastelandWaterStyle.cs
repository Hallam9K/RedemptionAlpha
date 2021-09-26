using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Waters
{
	public class WastelandWaterStyle : ModWaterStyle
	{

		public override int ChooseWaterfallStyle() => Find<ModWaterfallStyle>("Redemption/WastelandWaterfallStyle").Slot;

		public override int GetSplashDust() => Find<ModDust>("Redemption/Dusts/WastelandWaterSplash").Type; 

		public override int GetDropletGore() => Find<ModGore>("Redemption/Gores/Friendly/CoastScarabGore2").Type; //TODO replace with lab drop obviously

		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 1f;
			b = 0f;
		}

		public override Color BiomeHairColor()
		{
			return Color.LimeGreen;
		}
	}
}