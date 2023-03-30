using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class StormSummonerPro : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukko's Lightning");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 100;
        }

        int PosX;
        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.localAI[0] % 10 == 0 && Projectile.localAI[0] > 10)
                    PosX += 200;
                if (Projectile.localAI[0] % 10 == 0)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X - 1000 + PosX, Projectile.Center.Y), Projectile.velocity, ModContent.ProjectileType<UkkoStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Projectile.localAI[0] % 10 == 0 && Projectile.localAI[0] > 10)
                    PosX += 200;
                if (Projectile.localAI[0] % 10 == 0)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + 1000 - PosX, Projectile.Center.Y), Projectile.velocity, ModContent.ProjectileType<UkkoStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
            }
            else if (Projectile.ai[0] == 2)
            {
                if (Projectile.localAI[0] % 5 == 0 && Projectile.localAI[0] > 5)
                {
                    PosX += 200;
                }
                if (Projectile.localAI[0] % 5 == 0 && Projectile.localAI[0] <= 50)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X - 1000 + PosX, Projectile.Center.Y + 400), Projectile.velocity, ModContent.ProjectileType<UkkoStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + 1000 - PosX, Projectile.Center.Y - 400), Projectile.velocity, ModContent.ProjectileType<UkkoStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }
            }
            else if (Projectile.ai[0] == 3)
            {
                if (Projectile.localAI[0] == 2)
                {
                    for (int i = -6; i <= 6; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, 10 * Vector2.UnitX.RotatedBy(Math.PI / 6 * i), ModContent.ProjectileType<StormSummonerPro2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }
    }
}