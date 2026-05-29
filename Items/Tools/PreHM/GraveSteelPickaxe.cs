using Redemption.Items.Materials.PreHM;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PreHM
{
    public class GraveSteelPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.damage = 7;
            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 40;
            Item.useTime = 18;
            Item.useAnimation = 22;
            Item.pick = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = 1200;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GraveSteelAlloy>(), 10)
                .AddRecipeGroup(RecipeGroupID.Wood, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}