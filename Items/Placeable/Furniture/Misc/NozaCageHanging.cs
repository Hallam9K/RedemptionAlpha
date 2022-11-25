using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NozaCageHanging : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hanging Bastion Cage");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NozaCageHangingTile>(), 0);
			Item.width = 24;
			Item.height = 32;
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 0, 30, 0);
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.IronBar, 7)
				.AddIngredient(ItemID.Obsidian, 4)
				.AddIngredient(ItemID.Chain)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
}