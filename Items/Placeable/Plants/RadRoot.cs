using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Redemption.Items.Placeable.Plants
{
    public class RadRoot : ModItem
	{
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Grows in the Wasteland");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
		{
			Item.maxStack = Item.CommonMaxStack;
            Item.width = 16;
            Item.height = 20;
            Item.value = 150;
            Item.rare = ItemRarityID.Blue;
		}
	}
}