using Redemption.Items.Materials.PostML;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class NebBox2 : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Nebuleus' Final Form)");
            // Tooltip.SetDefault("musicman - Hypernova");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossStarGod2"), ModContent.ItemType<NebBox2>(), ModContent.TileType<NebBox2Tile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NebBox2Tile>(), 0);
			Item.createTile = ModContent.TileType<NebBox2Tile>();
			Item.width = 32;
			Item.height = 22;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<GildedStar>(), 20)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
