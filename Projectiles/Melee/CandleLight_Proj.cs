using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class CandleLight_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Candle Light");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            DrawOriginOffsetY = -8;
            Projectile.Redemption().Unparryable = true;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, Projectile.Opacity * 0.7f, Projectile.Opacity * 0.8f);
            Projectile.velocity *= 0.96f;
            if (Main.rand.NextBool(5))
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
                Main.dust[dustIndex].velocity.Y = -2f;
                Main.dust[dustIndex].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
        public override void Kill(int timeLeft)
        {
            RedeDraw.SpawnRing(Projectile.Center, Color.White, 0.1f, 0.9f, 2);
        }
    }
}