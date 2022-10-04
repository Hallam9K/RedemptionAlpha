using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Plants
{
    public class Nightshade : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'A purple plant the blooms in the night'");

			SacrificeTotal = 25;
		}
        public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.width = 16;
			Item.height = 20;
			Item.value = 150;
			Item.rare = ItemRarityID.Blue;
		}
	}
}