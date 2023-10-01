using System;
using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab.Volt
{
    public class Volt_OrbProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electric Orb");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.alpha -= 5;
            Projectile.rotation += 0.06f;
            Projectile.velocity.Y += 0.2f;
            if (Projectile.localAI[0]++ % 10 == 0 && Projectile.localAI[0] >= 10)
            {
                for (int m = 0; m < 16; m++)
                {
                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Frost);
                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(2f, 0f), m / (float)16 * 6.28f);
                    Main.dust[dustID].noGravity = true;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<StaticStunDebuff>(), 30);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, default, 1.5f);
                Main.dust[dustIndex].velocity *= 2;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 vel = Vector2.Normalize(Projectile.velocity);
                for (int i = 0; i < 8; ++i)
                {
                    vel = vel.RotatedBy(Math.PI / 4);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, vel * 3, ProjectileID.MartianTurretBolt, Projectile.damage / 2, 0f, Main.myPlayer);
                    Main.projectile[p].timeLeft = 60;
                    Main.projectile[p].tileCollide = false;
                    Main.projectile[p].netUpdate = true;
                }
            }
        }
    }
}