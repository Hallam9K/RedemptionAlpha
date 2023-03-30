using Redemption.Items.Materials.HM;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ammo
{
    public class BileArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Bile Arrow");
            // Tooltip.SetDefault("Decreases target's defense and drains life");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerArrow;
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
		{
			Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = 17;
            Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<BileArrow_Proj>();
            Item.shootSpeed = 4.25f;
            Item.ammo = AmmoID.Arrow;
		}
		public override void AddRecipes()
		{
            CreateRecipe(150)
                .AddIngredient(ItemID.WoodenArrow, 150)
                .AddIngredient(ModContent.ItemType<ToxicBile>())
                .AddTile(TileID.WorkBenches)
                .Register();
		}
	}
}
