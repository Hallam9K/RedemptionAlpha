using Redemption.Items.Materials.PreHM;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public static class RecipeSystem
    {
        public static void Load(Mod mod)
        {
            Redemption_AddRecipes(mod);
        }

        private static void Redemption_AddRecipes(Mod mod)
        {
            mod.CreateRecipe(ItemID.GreenDye)
                .AddIngredient<TreeBugShell>()
                .AddTile(TileID.DyeVat)
                .Register();

            mod.CreateRecipe(ItemID.CyanDye)
                .AddIngredient<CoastScarabShell>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}