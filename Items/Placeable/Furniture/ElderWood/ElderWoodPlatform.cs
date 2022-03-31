using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.ElderWood;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodPlatformTile>(), 0);
			Item.width = 24;
			Item.height = 14;
			Item.maxStack = 999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>())
				.AddTile(TileID.WorkBenches)
				.Register();
        }
	}
}