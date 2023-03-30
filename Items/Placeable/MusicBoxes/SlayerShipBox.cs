using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class SlayerShipBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Slayer's Crashed Ship)");
            // Tooltip.SetDefault("PeriTune - Suspense3");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SlayerShipMusic"), ModContent.ItemType<SlayerShipBox>(), ModContent.TileType<SlayerShipBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SlayerShipBoxTile>(), 0);
			Item.createTile = ModContent.TileType<SlayerShipBoxTile>();
			Item.width = 32;
			Item.height = 18;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<SlayerShipPanel2>(), 40)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
    }
}
