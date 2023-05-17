using Redemption.Tiles.Plants;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class PaleBrittlecap : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pale Brittlecap");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PaleBrittlecapTile>());
            Item.width = 20;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Gray;
		}
    }
    public class PaleBrittlecap2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pale Brittlecap (Big)");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PaleBrittlecapTile2>());
            Item.width = 30;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
