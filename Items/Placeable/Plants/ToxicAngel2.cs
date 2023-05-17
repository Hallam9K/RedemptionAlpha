using Redemption.Rarities;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class ToxicAngel2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Purified Toxic Angel");
            // Tooltip.SetDefault("'Tastes like jelly'");
            Item.ResearchUnlockCount = 5;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ToxicAngel2Tile>());
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
