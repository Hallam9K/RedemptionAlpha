using Terraria;
using Terraria.ModLoader;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Particles;
using Microsoft.Xna.Framework;

namespace Redemption.Projectiles.Melee
{
    public class XeniumLanceSpark_Proj : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Spark");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = 8;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DustHelper.DrawParticleElectricity<LightningParticle>(new Vector2(target.position.X, target.position.Y + Main.rand.Next(0, target.height)), new Vector2(target.TopRight.X, target.TopRight.Y + Main.rand.Next(0, target.height)), .5f, 10, 0.2f, 5);
            DustHelper.DrawParticleElectricity<LightningParticle>(new Vector2(target.TopRight.X, target.TopRight.Y + Main.rand.Next(0, target.height)), new Vector2(target.position.X, target.position.Y + Main.rand.Next(0, target.height)), .5f, 10, 0.2f, 5);

            Projectile.localNPCImmunity[target.whoAmI] = 7;
            target.immune[Projectile.owner] = 0;

            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 240);
        }
        public override void AI()
        {
            if (Main.rand.NextBool(6))
            {
                for (int i = 0; i < 2; i++)
                {
                    DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(Main.rand.Next(30, 61), RedeHelper.RandomRotation()), .5f, 10, 0.2f, 5);
                }
            }
        }
    }
}