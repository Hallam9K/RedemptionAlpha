using Redemption.Items.Materials.PostML;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class PZMusicBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Music Box (Patient Zero)");
            // Tooltip.SetDefault("Universe - Xenostate");
			Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabBossMusic2"), ModContent.ItemType<PZMusicBox>(), ModContent.TileType<PZMusicBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PZMusicBoxTile>(), 0);
			Item.createTile = ModContent.TileType<PZMusicBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<RawXenium>(), 5)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
    }
}
