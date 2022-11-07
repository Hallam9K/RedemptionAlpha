using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class HKMiniStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Statue of the Knight");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HKMiniStatueTile>(), 0);
            Item.width = 24;
            Item.height = 30;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 25, 0);
        }
    }
}