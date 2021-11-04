using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
	public class PetrifiedWoodBed : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodBedTile>(), 0);
			Item.width = 32;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 2000;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 15)
				.AddIngredient(ItemID.Silk, 5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}