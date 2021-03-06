using Redemption.Items.Materials.HM;
using Redemption.Tiles.MusicBoxes;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.MusicBoxes
{
	public class WastelandBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Music Box (Wasteland)");
			Tooltip.SetDefault("Musearys - The Wastelands");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland"), ModContent.ItemType<WastelandBox>(), ModContent.TileType<WastelandBoxTile>());
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<WastelandBoxTile>(), 0);
			Item.createTile = ModContent.TileType<WastelandBoxTile>();
			Item.width = 32;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MusicBox)
				.AddIngredient(ModContent.ItemType<XenomiteItem>(), 10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
