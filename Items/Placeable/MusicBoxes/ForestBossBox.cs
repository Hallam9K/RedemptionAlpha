using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.MusicBoxes;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
    public class ForestBossBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Cursed Beings of the Forest)");
			Tooltip.SetDefault("Peritune - Dramatic4");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossForest1"), ModContent.ItemType<ForestBossBox>(), ModContent.TileType<ForestBossBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ForestBossBoxTile>(), 0);
			Item.createTile = ModContent.TileType<ForestBossBoxTile>();
			Item.width = 32;
			Item.height = 24;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<AncientWood>(), 40)
				.AddRecipeGroup("Redemption:GathicStone", 20)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
