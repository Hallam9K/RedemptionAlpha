using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class BileLauncher : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Charges up and releases a stream of radioactive gloop\n" +
                "Uses Toxic Grenades as ammo"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 36;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.NPCDeath13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BileLauncher_Gloop>();
            Item.shootSpeed = 5;
            Item.useAmmo = ModContent.ItemType<ToxicGrenade>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<BileLauncher_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Xenomite>(15)
                .AddIngredient<ToxicBile>(10)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
