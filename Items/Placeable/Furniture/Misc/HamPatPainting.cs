using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class HamPatPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ham Pat");
            // Tooltip.SetDefault("'O. Tomato'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<HamPatPaintingTile>(), 0);
            Item.width = 54;
            Item.height = 54;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(0, 10, 0, 0);
        }
    }
}