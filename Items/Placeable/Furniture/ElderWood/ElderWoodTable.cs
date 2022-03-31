using Redemption.Tiles.Furniture.ElderWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodTable : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodTableTile>());
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 8)
				.AddTile(TileID.WorkBenches)
				.Register();
        }
	}
}