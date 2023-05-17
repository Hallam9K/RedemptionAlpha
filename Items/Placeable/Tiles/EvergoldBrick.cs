using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class EvergoldBrick : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EvergoldBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
