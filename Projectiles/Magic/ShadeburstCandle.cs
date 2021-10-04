using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.ID;

namespace Redemption.Projectiles.Magic
{
    public class ShadeburstCandle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadeburst Candle");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.alpha = 0;
            Projectile.usesLocalNPCImmunity = true;
        }

        public int ShootCount;
        Vector2 projPos;

        public override void AI()
        {
            ShootCount++;
            
            if (ShootCount > 60 && ShootCount < 120)
            {
                projPos = new(Projectile.Center.X, Projectile.Center.Y);
                if (ShootCount % 6 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), projPos, RedeHelper.PolarVector(4, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<ShadeburstProj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.frame = 1;
                }
            }

            if (ShootCount > 300 && ShootCount < 360)
            {
                projPos = new(Projectile.Center.X, Projectile.Center.Y);
                if (ShootCount % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), projPos, RedeHelper.PolarVector(4, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<ShadeburstProj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.frame = 2;
                }
            }

            if (ShootCount > 480 && ShootCount < 560)
            {
                projPos = new(Projectile.Center.X, Projectile.Center.Y);
                if (ShootCount % 4 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), projPos, RedeHelper.PolarVector(4, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<ShadeburstProj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.frame = 3;
                }
            }

            if (ShootCount > 680 && ShootCount < 760)
            {
                projPos = new(Projectile.Center.X, Projectile.Center.Y);
                if (ShootCount % 3 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), projPos, RedeHelper.PolarVector(4, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<ShadeburstProj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }

            if (ShootCount == 760)
            {
                Projectile.Kill();
            }
        }
    }
}