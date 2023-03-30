using Redemption.Items.Materials.PostML;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class NebBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Nebuleus)");
            // Tooltip.SetDefault("musicman - Interstellar Isolation");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossStarGod1"), ModContent.ItemType<NebBox>(), ModContent.TileType<NebBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NebBoxTile>(), 0);
			Item.createTile = ModContent.TileType<NebBoxTile>();
			Item.width = 32;
			Item.height = 22;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<GildedStar>(), 10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
