using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ammo
{
    public class AquaArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Makes targets wet");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerArrow;
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 2.5f;
            Item.value = 2;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<AquaArrow_Proj>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient(ItemID.WoodenArrow, 20)
                .AddIngredient(ItemID.Coral)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
