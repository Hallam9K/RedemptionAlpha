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
			Item.width = 24;
			Item.height = 14;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<AncientWoodPlatformTile>();
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