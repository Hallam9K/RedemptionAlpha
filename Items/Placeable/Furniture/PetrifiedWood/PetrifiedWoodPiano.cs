using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodPiano : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodPianoTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 4)
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 15)
				.AddIngredient(ItemID.Book)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}