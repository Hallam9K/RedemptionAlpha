using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodPiano : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodPianoTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 4)
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 15)
				.AddIngredient(ItemID.Book)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}