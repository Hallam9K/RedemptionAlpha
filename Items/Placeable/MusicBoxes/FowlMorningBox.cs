using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Tiles.MusicBoxes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class FowlMorningBox : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Music Box (Fowl Morning)");
            // Tooltip.SetDefault("Sc0p3r - Dawn of the Coop");
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
            Item.ResearchUnlockCount = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/FowlMorning"), ModContent.ItemType<FowlMorningBox>(), ModContent.TileType<FowlMorningBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<FowlMorningBoxTile>(), 0);
			Item.createTile = ModContent.TileType<FowlMorningBoxTile>();
			Item.width = 32;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<EggBomb>(), 30)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
