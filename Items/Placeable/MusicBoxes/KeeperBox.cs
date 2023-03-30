using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class KeeperBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (The Keeper)");
            // Tooltip.SetDefault("SpectralAves - Haunting Loneliness");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper"), ModContent.ItemType<KeeperBox>(), ModContent.TileType<KeeperBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<KeeperBoxTile>(), 0);
			Item.createTile = ModContent.TileType<KeeperBoxTile>();
			Item.width = 32;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<LostSoul>(), 3)
				.AddIngredient(ModContent.ItemType<GrimShard>(), 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
