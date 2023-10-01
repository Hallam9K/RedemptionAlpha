using Microsoft.Xna.Framework;
using Redemption.Base;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class HalPetSPEEN : ModProjectile
	{
        public override string Texture => "Redemption/Projectiles/Pets/HalPet";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SPEEN");
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
		{
            Projectile.width = 20;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
		}

        public override void AI()
        {
            if (Projectile.frame < 8)
            {
                Projectile.frame = 8;
            }
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 15)
                {
                    Projectile.frame = 8;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            int dustType = 58;
            for (int m = 0; m < 8; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 4f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(10f, 0f), m / (float)8 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            int dustType2 = 59;
            for (int m = 0; m < 10; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType2, 0f, 0f, 100, Color.White, 4f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(12f, 0f), m / (float)10 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            int dustType3 = 60;
            for (int m = 0; m < 12; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType3, 0f, 0f, 100, Color.White, 4f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(14f, 0f), m / (float)12 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            int dustType4 = 62;
            for (int m = 0; m < 14; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType4, 0f, 0f, 100, Color.White, 4f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(16f, 0f), m / (float)14 * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return !target.boss ? null : false;
        }
    }
}
