using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class WastelandCorruptUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/CorruptWastelandCavernBG");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/CorruptWastelandCavernBG");
		}
	}
	public class WastelandCrimsonUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
		{
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/CrimsonWastelandCavernBG");
			textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/CrimsonWastelandCavernBG");
		}
	}
}