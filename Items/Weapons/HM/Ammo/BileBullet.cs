using Redemption.Items.Materials.HM;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ammo
{
    public class BileBullet : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Bile Bullet");
            // Tooltip.SetDefault("Decreases target's defense and drains life");
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
		{
			Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 10;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 4f;
            Item.value = 7;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<BileBullet_Proj>();
            Item.shootSpeed = 5.25f;
            Item.ammo = AmmoID.Bullet;
		}

		public override void AddRecipes()
		{
            CreateRecipe(150)
                .AddIngredient(ItemID.MusketBall, 150)
                .AddIngredient(ModContent.ItemType<ToxicBile>())
                .AddTile(TileID.WorkBenches)
                .Register();
		}
	}
}
