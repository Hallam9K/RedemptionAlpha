using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodPlatform : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodPlatformTile>(), 0);
			Item.width = 24;
			Item.height = 14;
			Item.maxStack = 999;
		}

		public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>())
				.AddTile(TileID.WorkBenches)
				.Register();
        }
	}
}