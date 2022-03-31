using Redemption.Tiles.Furniture.ElderWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodLampTile>());
			Item.width = 12;
			Item.height = 34;
			Item.maxStack = 99;
			Item.value = 100;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Torch)
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}