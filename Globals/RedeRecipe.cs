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
			var recipe = mod.CreateRecipe(ItemID.GreenDye, 1);
			recipe.AddIngredient<Items.Materials.PreHM.TreeBugShell>();
			recipe.AddTile(TileID.DyeVat);
			recipe.Register(); 
			
			var recipe2 = mod.CreateRecipe(ItemID.CyanDye, 1);
			recipe2.AddIngredient<Items.Materials.PreHM.CoastScarabShell>();
			recipe2.AddTile(TileID.DyeVat);
			recipe2.Register();
		}
	}
}