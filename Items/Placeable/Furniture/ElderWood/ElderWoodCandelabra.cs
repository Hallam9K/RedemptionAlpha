using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodCandelabra : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodCandelabraTile>(), 0);
			Item.width = 24;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 300;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 5)
				.AddIngredient(ItemID.Torch, 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
