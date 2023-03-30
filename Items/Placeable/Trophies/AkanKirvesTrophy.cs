using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Trophies;

namespace Redemption.Items.Placeable.Trophies
{
    public class AkanKirvesTrophy : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AkanKirvesTrophyTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.Blue;
		}
	}
}