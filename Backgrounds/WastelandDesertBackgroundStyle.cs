using Terraria;
using Terraria.ModLoader;

namespace Redemption.Backgrounds
{
    public class WastelandDesertBackgroundStyle : ModSurfaceBackgroundStyle
    {
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandDesertBG3");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandDesertBG2");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Redemption/Backgrounds/WastelandDesertBG1");
		}
	}
}