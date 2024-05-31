using Microsoft.Xna.Framework;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class CorruptedDoubleRifle_Beam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Beam");
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
            Projectile.penetrate = 8;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.Redemption().EnergyBased = true;
        }
        private int timeLeftMax;
        public ref float Distance => ref Projectile.ai[1];
        public override void AI()
        {
            if (Projectile.localAI[0] % 2 == 0)
            {
                Vector2 v = Projectile.position;
                Color bright = Color.Multiply(new(255, 146, 135, 0), 1);
                Color mid = Color.Multiply(new(255, 62, 55, 0), 1);
                Color dark = Color.Multiply(new(150, 20, 54, 0), 1);

                Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1);
                Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1f);
                RedeParticleSystemManager.RedeQuadSystem.NewParticle(v, Vector2.Zero, new QuadParticle()
                {
                    StartColor = emberColor,
                    EndColor = glowColor,
                    Scale = new Vector2(.5f * ((float)Projectile.timeLeft / timeLeftMax))
                }, 10);
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 25;
                Distance = MathF.Max(Distance, 1f);
                float progress = 1 - Projectile.timeLeft / 700;
                float r = 0.992f;
                if (progress > 0.1)
                {
                    Projectile.velocity *= r;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.IndianRed, glowScale: 3);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.PlasmaBlast with { Volume = 0.5f }, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity / 3, Vector2.Zero, ModContent.ProjectileType<PlasmaRound_Blast>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
            player.RedemptionScreen().ScreenShakeIntensity += 3;
        }
    }
}
