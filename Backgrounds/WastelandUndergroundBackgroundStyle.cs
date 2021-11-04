using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
	public class WastelandUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{		
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandBiomeUG0"); 
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandBiomeUG1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandBiomeUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandBiomeUG3");
		}
	}
}