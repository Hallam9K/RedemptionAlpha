using Redemption.Items.Materials.PreHM;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
	public static class RedeRecipes
	{
		public static void Load(Mod mod)
		{
			Redemption_AddRecipes(mod);
		}

		private static void Redemption_AddRecipes(Mod mod)
		{
			var recipe = mod.CreateRecipe(ItemID.GreenDye);
			recipe.AddIngredient<TreeBugShell>();
			recipe.AddTile(TileID.DyeVat);
			recipe.Register(); 
			
			var recipe2 = mod.CreateRecipe(ItemID.CyanDye);
			recipe2.AddIngredient<CoastScarabShell>();
			recipe2.AddTile(TileID.DyeVat);
			recipe2.Register();
		}
	}
}