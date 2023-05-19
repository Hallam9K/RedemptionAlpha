using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Waters
{
	public class RuinedKingdomWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => Find<ModWaterfallStyle>("Redemption/RuinedKingdomWaterfallStyle").Slot;
        public override int GetSplashDust() => Find<ModDust>("Redemption/RuinedKingdomWaterSplash").Type;
        public override int GetDropletGore() => Find<ModGore>("Redemption/RuinedKingdomDroplet").Type;
        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = .5f;
            g = .8f;
            b = 1f;
        }
        public override Color BiomeHairColor()
        {
            return Color.Goldenrod;
        }
	}
}