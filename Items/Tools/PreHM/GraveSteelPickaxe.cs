using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;

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
			Item.damage = 7;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 18;
			Item.useAnimation = 22;
			Item.pick = 50;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.value = 1200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 10)
				.AddRecipeGroup(RecipeGroupID.Wood, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
}