using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class LabMusicBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Music Box (Abandoned Lab)");
            // Tooltip.SetDefault("inSignia - Facility of Contagion");
			Item.ResearchUnlockCount = 1;
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabMusic"), ModContent.ItemType<LabMusicBox>(), ModContent.TileType<LabMusicBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabMusicBoxTile>(), 0);
			Item.createTile = ModContent.TileType<LabMusicBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<LabPlating>(), 20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
    }
}
