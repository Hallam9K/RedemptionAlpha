using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class XeniumRefinery : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Used to craft Xenium"
                + "\nFound in the Abandoned Laboratory"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<XeniumRefineryTile>(), 0);
            Item.width = 30;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Purple;
        }
    }
}