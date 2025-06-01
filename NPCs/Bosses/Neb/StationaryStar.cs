using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class StationaryStar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Starplane");
            Main.projFrames[Projectile.type] = 6;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 56;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 160;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NebSound3, Projectile.position);
            target.RedemptionScreen().ScreenShakeIntensity += 70;
            for (int d = 0; d < 16; d++)
                RedeParticleManager.CreateRainbowParticle(Projectile.Center, RedeHelper.Spread(6), Main.rand.NextFloat(.4f, 1f));
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback.Base += 6;
        }
        public override void OnKill(int timeLeft)
        {
            for (int m = 0; m < 8; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)8 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
    }

}
