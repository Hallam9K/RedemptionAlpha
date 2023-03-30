using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class ErhanBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Erhan)");
            // Tooltip.SetDefault("Sc0p3r - Holy Inquisition");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossErhan"), ModContent.ItemType<ErhanBox>(), ModContent.TileType<ErhanBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ErhanBoxTile>(), 0);
			Item.createTile = ModContent.TileType<ErhanBoxTile>();
			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<Archcloth>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
