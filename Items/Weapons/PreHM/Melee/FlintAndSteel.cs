using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class FlintAndSteel : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flint and Steel");
            /* Tooltip.SetDefault("Releases a tiny spark which lights enemies on fire\n" +
                "'Doesn't work on obsidian'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Generic;
            Item.width = 30;
            Item.height = 24;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = 2800;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item17;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FlintAndSteelSpark>();
            Item.shootSpeed = 6;
        }
    }
    public class FlintAndSteelSpark : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spark");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 12;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Torch, Projectile.velocity.X, Projectile.velocity.Y, 200, Scale: 1.5f);
            dust.velocity += Projectile.velocity * 0.3f;
            dust.velocity *= 0.2f;
            dust.noGravity = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }
    }
}