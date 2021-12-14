using Redemption.Items.Accessories.HM;
using Redemption.Items.Critters;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeRecipe : ModSystem
    {
        public static RecipeGroup ChickenRecipeGroup;
        public static RecipeGroup GoldRecipeGroup;
        public static RecipeGroup SilverRecipeGroup;
        public static RecipeGroup CopperRecipeGroup;
        public static RecipeGroup GathicStoneRecipeGroup;
        public static RecipeGroup BioweaponBileRecipeGroup;
        public static RecipeGroup HazmatSuitRecipeGroup;
        public static RecipeGroup PlantRecipeGroup;

        public override void Unload()
        {
            ChickenRecipeGroup = null;
            GoldRecipeGroup = null;
            SilverRecipeGroup = null;
            CopperRecipeGroup = null;
            GathicStoneRecipeGroup = null;
            BioweaponBileRecipeGroup = null;
            HazmatSuitRecipeGroup = null;
            PlantRecipeGroup = null;
        }

        public override void AddRecipeGroups()
        {
            ChickenRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<ChickenItem>())}", ModContent.ItemType<ChickenItem>(), ModContent.ItemType<RedChickenItem>(), ModContent.ItemType<LeghornChickenItem>(), ModContent.ItemType<BlackChickenItem>());
            RecipeGroup.RegisterGroup("Redemption:Chickens", ChickenRecipeGroup);

            GoldRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("Redemption:GoldBar", GoldRecipeGroup);

            SilverRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.IronBar)}", ItemID.TungstenBar, ItemID.SilverBar);
            RecipeGroup.RegisterGroup("Redemption:SilverBar", SilverRecipeGroup);

            CopperRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", ItemID.CopperBar, ItemID.TinBar);
            RecipeGroup.RegisterGroup("Redemption:CopperBar", CopperRecipeGroup);

            GathicStoneRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<GathicStone>())}", ModContent.ItemType<GathicStone>(), ModContent.ItemType<GathicGladestone>(), ModContent.ItemType<GathicStoneBrick>(), ModContent.ItemType<GathicGladestoneBrick>());
            RecipeGroup.RegisterGroup("Redemption:GathicStone", GathicStoneRecipeGroup);

            BioweaponBileRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<Bioweapon>())}", ModContent.ItemType<Bioweapon>(), ModContent.ItemType<ToxicBile>());
            RecipeGroup.RegisterGroup("Redemption:BioweaponBile", BioweaponBileRecipeGroup);

            HazmatSuitRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<HazmatSuit>())}", ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<HazmatSuit2>(), ModContent.ItemType<HazmatSuit3>());
            RecipeGroup.RegisterGroup("Redemption:HazmatSuits", HazmatSuitRecipeGroup);

            PlantRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Daybloom)}", ItemID.Daybloom, ItemID.Waterleaf, ItemID.Blinkroot, ItemID.Deathweed, ItemID.Fireblossom, ItemID.Moonglow, ItemID.Shiverthorn, ModContent.ItemType<Nightshade>());
            RecipeGroup.RegisterGroup("Redemption:Plants", PlantRecipeGroup);
        }

        public override void AddRecipes()
        {
            Mod.CreateRecipe(ItemID.GreenDye)
                .AddIngredient<TreeBugShell>()
                .AddTile(TileID.DyeVat)
                .Register();

            Mod.CreateRecipe(ItemID.CyanDye)
                .AddIngredient<CoastScarabShell>()
                .AddTile(TileID.DyeVat)
                .Register();

            // Living Furniture
            Mod.CreateRecipe(ItemID.LivingLoom)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LeafWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingLeafWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodDoor)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodChair)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodTable)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodPiano)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodBookcase)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodBed)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodSofa)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodBathtub)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 14)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodLantern)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodLamp)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 3)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodCandle)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodChandelier)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient(ItemID.Chain)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodCandelabra)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodWorkBench)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodPlatform, 2)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodClock)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddRecipeGroup("IronBar", 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Mod.CreateRecipe(ItemID.LivingWoodSink)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
            // --------------------
        }
    }
}