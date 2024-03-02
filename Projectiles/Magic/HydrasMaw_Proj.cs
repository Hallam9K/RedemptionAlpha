using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class HydrasMaw_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            ElementID.ProjWater[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
        }
        public override void AI()
        {
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
                Projectile.Kill();
            Projectile.velocity.Y += 0.075f;
            for (int i = 0; i < 3; i++)
            {
                float x = Projectile.velocity.X / 3f * i;
                float y = Projectile.velocity.Y / 3f * i;
                int size = 10;
                int dust = Dust.NewDust(Projectile.position, Projectile.width - (size * 2), Projectile.height - size * 2, ModContent.DustType<HydraAcidDust>(), 0f, 0f, 100);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].velocity += Projectile.velocity * 0.5f;
                Dust dust2 = Main.dust[dust];
                dust2.position.X -= x;
                Dust dust3 = Main.dust[dust];
                dust3.position.Y -= y;
            }
            if (Main.rand.NextBool(8))
            {
                int size = 10;
                int num157 = Dust.NewDust(Projectile.position, Projectile.width - size * 2, Projectile.height - size * 2, ModContent.DustType<HydraAcidDust>(), 0f, 0f, 100, default, 0.5f);
                Main.dust[num157].velocity *= 0.25f;
                Main.dust[num157].velocity += Projectile.velocity * 0.5f;
                return;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HydraAcidDebuff>(), 600);
    }
}