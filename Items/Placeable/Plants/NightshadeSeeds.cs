using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Plants;

namespace Redemption.Items.Placeable.Plants
{
    public class NightshadeSeeds : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 25;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NightshadeTile>(), 0);
			Item.maxStack = Item.CommonMaxStack;
			Item.width = 12;
			Item.height = 14;
			Item.value = 80;
			Item.rare = ItemRarityID.Blue;
		}
	}
}