using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class NoidanSauva : ModItem
	{
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("A witch's staff that shoots enchanting spark projectiles");
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
			Item.damage = 38;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 4;
            Item.useAnimation = 8;
            Item.reuseDelay = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(0, 6, 75, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<NoidanNuoli>();
            Item.shootSpeed = 18f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }
    }
}
