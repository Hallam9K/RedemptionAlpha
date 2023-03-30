using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class DoubleRifle : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Double Rifle");
            /* Tooltip.SetDefault("Converts normal bullets into high velocity bullets\n" +
                "33% chance not to consume ammo"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 56;
            Item.height = 28;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 90;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= .33f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ProjectileID.BulletHighVelocity;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(4, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + RedeHelper.PolarVector(4, (player.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
