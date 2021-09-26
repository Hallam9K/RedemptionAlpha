using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
	public class LabUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{		
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/LabBiomeUG0"); 
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/LabBiomeUG1");
			textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/LabBiomeUG2");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/LabBiomeUG3");
		}
	}
}