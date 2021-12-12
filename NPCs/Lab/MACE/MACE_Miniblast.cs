using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace Redemption.NPCs.Lab.MACE
{
    public class MACE_Miniblast : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flaming Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
		{
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 160;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.OrangeTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust1].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 1f, Projectile.Opacity * 1f, Projectile.Opacity * 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14.WithVolume(.2f), Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Scale: 2f);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
    }
}
