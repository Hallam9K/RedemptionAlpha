using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class XenoSplat : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] == 1)
                return Color.White * Projectile.Opacity;
            return base.GetAlpha(lightColor);
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.Center - new Vector2(10), 20, 20, DustID.GreenBlood);
                    Main.dust[dustIndex].velocity *= 3f;
                }

                if (Projectile.ai[2] == 0)
                {
                    switch (Main.rand.Next(3))
                    {
                        default:
                            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(SoundID.NPCDeath12, Projectile.position);
                            break;
                        case 2:
                            SoundEngine.PlaySound(SoundID.NPCDeath21, Projectile.position);
                            break;
                    }
                }
                Projectile.ai[0] = 1;
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.Kill();
            }
        }
    }
}