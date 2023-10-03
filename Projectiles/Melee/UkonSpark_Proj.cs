using Terraria;
using Terraria.ModLoader;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;

namespace Redemption.Projectiles.Melee
{
    public class UkonSpark_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukon Spark");
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 120;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                for (int i = 0; i < 2; i++)
                    DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center + RedeHelper.Spread(80), Projectile.Center + RedeHelper.Spread(80), 0.5f, 10, 0.2f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 300);
        }
    }
}