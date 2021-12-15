using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class WastelandSnowUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{		
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SnowWastelandUGBG1"); 
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SnowWastelandUGBG2");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SnowWastelandCavernBG1");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SnowWastelandCavernBG2");
		}
	}
}