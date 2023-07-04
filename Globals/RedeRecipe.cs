using Redemption.Items.Accessories.HM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Usable.Potions;
using Redemption.Items.Weapons.PreHM.Ranged;
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
        public static RecipeGroup HazmatSuitRecipeGroup;
        public static RecipeGroup PlantRecipeGroup;

        public override void Unload()
        {
            ChickenRecipeGroup = null;
            GoldRecipeGroup = null;
            SilverRecipeGroup = null;
            CopperRecipeGroup = null;
            GathicStoneRecipeGroup = null;
            HazmatSuitRecipeGroup = null;
            PlantRecipeGroup = null;
        }

        public override void AddRecipeGroups()
        {
            ChickenRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<ChickenItem>())}", ModContent.ItemType<ChickenItem>(), ModContent.ItemType<RedChickenItem>(), ModContent.ItemType<LeghornChickenItem>(), ModContent.ItemType<BlackChickenItem>());
            RecipeGroup.RegisterGroup("Redemption:Chickens", ChickenRecipeGroup);

            GoldRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.GoldBar, ItemID.PlatinumBar);
            RecipeGroup.RegisterGroup("Redemption:GoldBar", GoldRecipeGroup);

            SilverRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}", ItemID.SilverBar, ItemID.TungstenBar);
            RecipeGroup.RegisterGroup("Redemption:SilverBar", SilverRecipeGroup);

            CopperRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.CopperBar)}", ItemID.CopperBar, ItemID.TinBar);
            RecipeGroup.RegisterGroup("Redemption:CopperBar", CopperRecipeGroup);

            GathicStoneRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<GathicStone>())}", ModContent.ItemType<GathicStone>(), ModContent.ItemType<GathicGladestone>(), ModContent.ItemType<GathicStoneBrick>(), ModContent.ItemType<GathicGladestoneBrick>());
            RecipeGroup.RegisterGroup("Redemption:GathicStone", GathicStoneRecipeGroup);

            HazmatSuitRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ModContent.ItemType<HazmatSuit>())}", ModContent.ItemType<HazmatSuit>(), ModContent.ItemType<HazmatSuit2>(), ModContent.ItemType<HazmatSuit3>());
            RecipeGroup.RegisterGroup("Redemption:HazmatSuits", HazmatSuitRecipeGroup);

            PlantRecipeGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.Daybloom)}", ItemID.Daybloom, ItemID.Waterleaf, ItemID.Blinkroot, ItemID.Deathweed, ItemID.Fireblossom, ItemID.Moonglow, ItemID.Shiverthorn, ModContent.ItemType<Nightshade>());
            RecipeGroup.RegisterGroup("Redemption:Plants", PlantRecipeGroup);

            RecipeGroup.RegisterGroup("Fruit", new RecipeGroup(null, ModContent.ItemType<Olives>(), ModContent.ItemType<Avocado>()));
            RecipeGroup.RegisterGroup("Sand", new RecipeGroup(null, ModContent.ItemType<IrradiatedSand>(), ModContent.ItemType<IrradiatedHardenedSand>()));
            RecipeGroup.RegisterGroup("Snails", new RecipeGroup(null, ModContent.ItemType<JohnSnailItem>()));
            RecipeGroup.RegisterGroup("Wood", new RecipeGroup(null, ModContent.ItemType<ElderWood>(), ModContent.ItemType<PetrifiedWood>()));
        }
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                if (recipe.HasResult(ItemID.Zenith))
                    recipe.AddIngredient<LifeFragment>(10);
            }
        }
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.WizardHat)
                .AddCustomShimmerResult(ModContent.ItemType<DruidHat>())
                .Register()
                .DisableRecipe();

            Recipe.Create(ItemID.GreenDye)
                .AddIngredient<TreeBugShell>()
                .AddTile(TileID.DyeVat)
                .Register();

            Recipe.Create(ItemID.CyanDye)
                .AddIngredient<CoastScarabShell>()
                .AddTile(TileID.DyeVat)
                .Register();
            
            Recipe.Create(ItemID.Escargot)
                .AddIngredient<JohnSnailItem>()
                .AddTile(TileID.CookingPots)
                .Register();

            Recipe.Create(ItemID.RottenEgg, 15)
                .AddIngredient<ChickenEgg>(15)
                .AddIngredient(ItemID.VilePowder)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.RottenEgg, 15)
                .AddIngredient<ChickenEgg>(15)
                .AddIngredient(ItemID.ViciousPowder)
                .AddTile(TileID.WorkBenches)
                .Register();

            Recipe.Create(ItemID.BunnyStew)
                .AddIngredient<HazmatBunnyItem>()
                .AddTile(TileID.CookingPots)
                .Register();

            Recipe.Create(ItemID.DryadCoverings)
                .AddIngredient(ItemID.Vine, 6)
                .AddTile(TileID.Loom)
                .Register();
            Recipe.Create(ItemID.DryadLoincloth)
                .AddIngredient(ItemID.Vine, 4)
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.NightOwlPotion)
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Nightshade>()
                .AddTile(TileID.Bottles)
                .Register();

            Recipe.Create(ItemID.SlimeStaff)
                .AddIngredient<ElderWood>(12)
                .AddIngredient(ItemID.Gel, 25)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.WoodenArrow, 25)
                .AddIngredient<ElderWood>()
                .AddRecipeGroup(GathicStoneRecipeGroup)
                .AddTile(TileID.WorkBenches)
                .Register();

            Recipe.Create(ItemID.ThrowingKnife, 33)
                .AddIngredient<GraveSteelAlloy>()
                .AddRecipeGroup(RecipeGroupID.Wood)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ChainKnife)
                .AddIngredient<GraveSteelAlloy>(9)
                .AddIngredient(ItemID.Chain, 3)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.AncientChisel)
                .AddIngredient<GraveSteelAlloy>(4)
                .AddIngredient<ElderWood>(20)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.Marrow)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient<GraveSteelAlloy>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            Recipe.Create(ItemID.HerbBag)
                .AddIngredient(ItemID.Leather, 4)
                .AddIngredient<PlantMatter>(20)
                .AddTile(TileID.WorkBenches)
                .Register();

            // Living Furniture
            Recipe.Create(ItemID.LivingLoom)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingMahoganyWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddIngredient(ItemID.RichMahogany, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LeafWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingMahoganyLeafWand)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 12)
                .AddIngredient(ItemID.RichMahogany, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingLeafWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodWall, 4)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodDoor)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodChair)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodTable)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodPiano)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodBookcase)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodBed)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodSofa)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodBathtub)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 14)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodLantern)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodLamp)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 3)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodCandle)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodChandelier)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient(ItemID.Chain)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodCandelabra)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 5)
                .AddIngredient(ItemID.Torch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodWorkBench)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodPlatform, 2)
                .AddIngredient(ModContent.ItemType<LivingTwig>())
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodClock)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 10)
                .AddRecipeGroup(RecipeGroupID.IronBar, 3)
                .AddIngredient(ItemID.Glass, 6)
                .AddTile(TileID.WorkBenches)
                .Register();
            Recipe.Create(ItemID.LivingWoodSink)
                .AddIngredient(ModContent.ItemType<LivingTwig>(), 6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.WorkBenches)
                .Register();
            // --------------------
        }
    }
}