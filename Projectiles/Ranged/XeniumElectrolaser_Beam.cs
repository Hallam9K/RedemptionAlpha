using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class XeniumElectrolaser_Beam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Laser");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 700;
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().EnergyBased = true;
        }
        public override void AI()
        {
            Projectile.width = Projectile.height = 4 + ((int)Projectile.ai[0] / 2);
            if (Projectile.localAI[0]++ > 5)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 v = Projectile.position;
                    v -= Projectile.velocity * (i * 0.25f);
                    ParticleManager.NewParticle(v, Vector2.Zero, new LightningParticle(), Color.White, 1 + Projectile.ai[0] / 10, 5);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 100;
            target.immune[Projectile.owner] = 0;
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.LightGreen);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast with { Volume = 0.5f }, Projectile.position);
            player.RedemptionScreen().ScreenShakeIntensity += Projectile.ai[0];
        }
    }
    public class XeniumElectrolaser_Beam2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Laser");
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 700;
            Projectile.penetrate = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.Redemption().EnergyBased = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] < 1)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 v = Projectile.position;
                    v -= Projectile.velocity * (i * 0.25f);
                    ParticleManager.NewParticle(v, Vector2.Zero, new LightningParticle(), Color.White, 0.7f, 5, 1);
                }
            }
        }
    }
}
