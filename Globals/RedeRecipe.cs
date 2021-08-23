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

            // Living Furniture
            mod.CreateRecipe(ItemID.LivingLoom)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LeafWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingLeafWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodDoor)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodChair)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodTable)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodPiano)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodBookcase)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodBed)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodSofa)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodBathtub)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 14)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodLantern)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodLamp)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 3)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodCandle)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodChandelier)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient(ItemID.Chain)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodCandelabra)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodWorkBench)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodPlatform, 2)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodClock)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddRecipeGroup("IronBar", 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            mod.CreateRecipe(ItemID.LivingWoodSink)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
            // --------------------
        }
    }
}