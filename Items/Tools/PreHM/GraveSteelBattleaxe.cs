using Redemption.Items.Materials.PreHM;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PreHM
{
    public class GraveSteelBattleaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.width = 50;
            Item.height = 44;
            Item.useTime = 17;
            Item.axe = 11;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = 1100;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GraveSteelAlloy>(), 8)
                .AddRecipeGroup(RecipeGroupID.Wood, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
