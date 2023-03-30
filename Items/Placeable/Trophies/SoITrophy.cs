using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Tiles.Trophies;

namespace Redemption.Items.Placeable.Trophies
{
    public class SoITrophy : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Seed of Infection Trophy");

			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SoITrophyTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 1, 33, 0);
			Item.rare = ItemRarityID.Blue;
		}
	}
}