using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class PrisonBars : ModItem
	{
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<PrisonBarsTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}
