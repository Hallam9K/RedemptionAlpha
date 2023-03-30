using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class ForestBossBox2 : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Ancient Deity Duo)");
            // Tooltip.SetDefault("Peritune - Havoc");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest2"), ModContent.ItemType<ForestBossBox2>(), ModContent.TileType<ForestBossBoxTile2>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ForestBossBoxTile2>(), 0);
			Item.createTile = ModContent.TileType<ForestBossBoxTile2>();
			Item.width = 32;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ItemID.LunarBar, 5)
				.AddIngredient(ModContent.ItemType<ElderWood>(), 40)
				.AddRecipeGroup(RedeRecipe.GathicStoneRecipeGroup, 20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
