using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Placeable.Plants
{
    public class Nightshade : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'A purple plant the blooms in the night'");
        }
        public override void SetDefaults()
		{
			Item.maxStack = 99;
			Item.width = 16;
			Item.height = 20;
			Item.value = 150;
			Item.rare = ItemRarityID.Blue;
		}
	}
}