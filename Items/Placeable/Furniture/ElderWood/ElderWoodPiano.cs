using Redemption.Tiles.Furniture.ElderWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodPiano : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodPianoTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 4)
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 15)
				.AddIngredient(ItemID.Book)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}