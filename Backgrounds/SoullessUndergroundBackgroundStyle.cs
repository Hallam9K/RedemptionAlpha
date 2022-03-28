using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class SoullessUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{		
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SoullessCaveUG0"); 
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SoullessCaveUG1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SoullessCaveUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/SoullessCaveUG3");
		}
	}
}