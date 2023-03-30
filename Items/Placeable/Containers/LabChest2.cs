using Redemption.Tiles.Containers;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class LabChest2 : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Special Laboratory Crate");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabChestTileLocked2>(), 0);
			Item.width = 30;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 5000;
			Item.rare = ItemRarityID.LightPurple;
		}
    }
}