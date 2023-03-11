using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class XeniumSmelter : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to melt scraps of metal\n" +
                "Counts as a forge"
                + "\nFound in the Abandoned Laboratory");
			SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<XeniumSmelterTile>(), 0);
			Item.width = 30;
			Item.height = 36;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Purple;
		}
	}
}