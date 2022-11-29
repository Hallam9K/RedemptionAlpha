using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Waters
{
	public class WastelandWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => Find<ModWaterfallStyle>("Redemption/WastelandWaterfallStyle").Slot;

		public override int GetSplashDust() => Find<ModDust>("Redemption/WastelandWaterSplash").Type; 

		public override int GetDropletGore() => Find<ModGore>("Redemption/XenoDroplet").Type;

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
		public override byte GetRainVariant()
		{
			return (byte)Main.rand.Next(3);
		}
		public override Asset<Texture2D> GetRainTexture()
		{
			return Request<Texture2D>("Redemption/Water/WastelandRain");
		}
	}
}