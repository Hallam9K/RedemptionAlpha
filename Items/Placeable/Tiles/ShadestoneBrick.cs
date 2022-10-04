using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneBrick : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}
