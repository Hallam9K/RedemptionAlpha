using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class WaxCandles : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WaxCandlesTile>(), 0);
            Item.width = 14;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue; 
        }
    }
}