using Redemption.Tiles.Plants;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class PaleBrittlecap : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pale Brittlecap");
            SacrificeTotal = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PaleBrittlecapTile>());
            Item.width = 20;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Gray;
		}
    }
    public class PaleBrittlecap2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pale Brittlecap (Big)");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PaleBrittlecapTile2>());
            Item.width = 30;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
