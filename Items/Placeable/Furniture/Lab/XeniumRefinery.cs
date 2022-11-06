using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class XeniumRefinery : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to craft Xenium"
                + "\nFound in the Abandoned Laboratory");
			SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<XeniumRefineryTile>(), 0);
			Item.width = 30;
			Item.height = 36;
			Item.maxStack = 9999;
			Item.value = Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Purple;
		}
	}
}