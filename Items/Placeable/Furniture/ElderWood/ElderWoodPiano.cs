using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodPiano : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodPianoTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
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