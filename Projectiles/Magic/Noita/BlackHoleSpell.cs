using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class BlackHoleSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Black Hole");
            Main.projFrames[Projectile.type] = 13;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjShadow[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 30)
            {
                if (Projectile.frame < 9)
                    Projectile.frame = 9;
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 13)
                        Projectile.Kill();
                }
                return;
            }
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 10)
                    Projectile.frame = 0;
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || Projectile.DistanceSQ(npc.Center) >= 100 * 100 || npc.boss || npc.knockBackResist == 0 || npc.friendly || npc.lifeMax > 4000)
                    continue;

                float num3 = 10f;
                float x = Projectile.Center.X - npc.Center.X;
                float y = Projectile.Center.Y - npc.Center.Y;
                float num6 = (float)Math.Sqrt(x * x + y * y);
                num6 = num3 / num6;
                x *= num6;
                y *= num6;
                int succPower;
                if (Projectile.DistanceSQ(npc.Center) < 20 * 20)
                    succPower = 15;
                else if (Projectile.DistanceSQ(npc.Center) < 50 * 50)
                    succPower = 18;
                else if (Projectile.DistanceSQ(npc.Center) < 75 * 75)
                    succPower = 20;
                else
                    succPower = 30;
                npc.velocity.X = (npc.velocity.X * (succPower - 1) + x) / succPower;
                npc.velocity.Y = (npc.velocity.Y * (succPower - 1) + y) / succPower;
            }
            for (int i = 0; i < 15; i++)
            {
                float distance = Main.rand.Next(14) * 4;
                Vector2 dustRotation = new Vector2(distance, 0f).RotatedBy(MathHelper.ToRadians(i * 24));
                Vector2 dustPosition = Projectile.Center + dustRotation;
                Vector2 nextDustPosition = Projectile.Center + dustRotation.RotatedBy(MathHelper.ToRadians(-4));
                Vector2 dustVelocity = dustPosition - nextDustPosition + Projectile.velocity;
                if (Main.rand.NextBool(6))
                {
                    Dust dust = Dust.NewDustPerfect(dustPosition, ModContent.DustType<BlackHoleDust>(), dustVelocity, Scale: 0.2f);
                    dust.scale = distance / 30;
                    dust.scale = MathHelper.Clamp(dust.scale, 0.2f, 1);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.alpha += 10;
                    dust.rotation = dustRotation.ToRotation();
                }
            }
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BlackHoleDust>());
                Main.dust[dust].noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
    }
}