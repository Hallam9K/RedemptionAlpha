using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class ShadowBolt : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Bolt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int m = 0; m < 8; m++)
                {
                    int dustID = Dust.NewDust(Projectile.Center - new Vector2(-1, -1), 2, 2, DustID.ShadowbeamStaff, 0f, 0f, 100, Color.White, 2f);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)8 * 6.28f);
                    Main.dust[dustID].noLight = false;
                    Main.dust[dustID].noGravity = true;
                }
                Projectile.localAI[0] = 1f;
            }
            int dust = Dust.NewDust(Projectile.Center - new Vector2(-2, -2), 4, 4, DustID.RainbowRod,
                Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, new Color(131, 0, 0), 2f);
            Main.dust[dust].velocity *= 0.5f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].noLight = false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            Projectile.penetrate--;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int m = 0; m < 8; m++)
            {
                int dustID = Dust.NewDust(Projectile.Center - new Vector2(-1, -1), 2, 2, DustID.ShadowbeamStaff, 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)8 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
    }
}