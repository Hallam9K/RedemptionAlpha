using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class WastelandUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{		
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandUGBG1"); 
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandUGBG2");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandCavernBG1");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandCavernBG2");
		}
	}
}