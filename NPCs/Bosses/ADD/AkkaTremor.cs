using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class AkkaTremor : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Earth Tremor");
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            Projectile.velocity.Y = 0;
            Projectile.velocity.X = 0;
            if (Projectile.localAI[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item89, Projectile.position);
                for (int i = 0; i < 50; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }
    }
}