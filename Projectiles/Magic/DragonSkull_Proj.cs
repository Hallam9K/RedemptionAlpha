using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class DragonSkull_Proj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (player.channel && Projectile.ai[1] == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.rotation.SlowRotation(Main.MouseWorld.ToRotation(), (float)Math.PI / 80);
                if (Projectile.ai[0]++ == 0)
                {
                    for (int i = 0; i < 20; i++)
                        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2);
                }
                if (Projectile.ai[0] == 40)
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.position);
                if (Projectile.ai[0] >= 40 && Projectile.ai[0] % 3 == 0 && Projectile.ai[0] <= 180)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(8, Projectile.rotation), ProjectileID.Flames, Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (Projectile.ai[0] == 280)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(10, Projectile.rotation), ProjectileID.Flames, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            else
            {
                Projectile.ai[1] = 1;
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
    }
}