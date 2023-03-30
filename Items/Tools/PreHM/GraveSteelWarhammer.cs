using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Tools.PreHM
{
    public class GraveSteelWarhammer : ModItem
    {
        public override void SetStaticDefaults()
        {
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 32;
			Item.useTime = 23;
			Item.useAnimation = 38;
			Item.hammer = 50;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7;
			Item.value = 1050;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

        public override void AddRecipes() //the Recipe of the item
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 8)
				.AddRecipeGroup(RecipeGroupID.Wood, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
}