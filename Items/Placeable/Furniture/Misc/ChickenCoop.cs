using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ChickenCoop : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("Occasionally spawns chicken eggs");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 42;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.Blue;
			Item.consumable = true;
			Item.value = Item.sellPrice(0, 0, 15, 0);
			Item.createTile = ModContent.TileType<ChickenCoopTile>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup("Redemption:Chickens", 2)
				.AddRecipeGroup(RecipeGroupID.Wood, 30)
				.AddIngredient(ItemID.Hay, 10)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}