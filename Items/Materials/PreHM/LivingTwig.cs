using Terraria;
using Terraria.ModLoader;


namespace Redemption.Items.Materials.PreHM
{
    public class LivingTwig : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("'It's moving..." +
				"\nOh nevermind, it's just the wind.'"); */

			Item.ResearchUnlockCount = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 0, 0, 8);
		}
    }
}
