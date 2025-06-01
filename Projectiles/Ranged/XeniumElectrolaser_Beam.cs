using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            timeLeftMax = Projectile.timeLeft;
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
            Projectile.Redemption().EnergyBased = true;
        }
        private int timeLeftMax;
        public override void OnSpawn(IEntitySource source)
        {
            DrawParticle();
        }
        public override void AI()
        {
            Projectile.width = Projectile.height = 4 + ((int)Projectile.ai[0] / 2);

            if (Projectile.localAI[0]++ >= 15f && Projectile.localAI[0] % 10 == 0)
            {
                Color bright = Color.Multiply(new(186, 255, 185, 0), 1);
                Color dark = Color.Multiply(new(23, 165, 107, 0), 1);

                Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1);
                float Scale = (1 + Projectile.ai[0] / 10);
                float squish = 4;
                if (Projectile.ai[0] != 1)
                    squish = 3;

                Vector2 drawPos = Projectile.Center + Projectile.velocity * Projectile.ai[0];
                RedeParticleManager.CreateLaserParticle(drawPos, Projectile.velocity, Scale * 2, emberColor, squish);
            }
        }
        public void DrawParticle()
        {
            float angle = (Projectile.Center - Main.MouseWorld).ToRotation();
            Vector2 position = Projectile.Center;
            Color bright = Color.Multiply(new(186, 255, 185, 0), 1);
            int num1 = Projectile.ai[0] == 1 ? 2 : 8;
            int num2 = Projectile.ai[0] == 1 ? 2 : 4;
            for (int j = 0; j < num1; j++)
            {
                float randomRotation = Main.rand.NextFloat(-0.4f, 0.4f);
                float randomVel = Main.rand.NextFloat(1.5f, 3);
                RedeParticleManager.CreateSpeedParticle(position, Projectile.velocity.RotatedBy(randomRotation) * randomVel * 3 * num2, 1, bright.WithAlpha(0));
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
            Projectile.extraUpdates = 50;
            Projectile.timeLeft = 700;
            timeLeftMax = Projectile.timeLeft;
            Projectile.penetrate = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.Redemption().EnergyBased = true;
        }
        private int timeLeftMax;
        public override void AI()
        {
            if (Projectile.localAI[0]++ >= 2f && Projectile.localAI[0] % 5 == 0)
            {
                Color bright = Color.Multiply(new(186, 255, 185, 0), 1);
                Color dark = Color.Multiply(new(23, 165, 107, 0), 1);

                Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1);

                Vector2 drawPos = Projectile.Center;
                RedeParticleManager.CreateLaserParticle(drawPos, Projectile.velocity, 1, emberColor, 2);
            }
        }
    }
}
