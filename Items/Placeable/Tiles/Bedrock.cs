using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class Bedrock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BedrockTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.value = 50;
        }
    }
}
