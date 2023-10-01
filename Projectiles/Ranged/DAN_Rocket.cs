using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class DAN_Rocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("D.A.N Rocket");
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }
        NPC target;
        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Scale: 2);
            d.noGravity = true;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[0] == 1 && Projectile.timeLeft < 285)
            {
                if (RedeHelper.ClosestNPC(ref target, 400, Projectile.Center, true))
                    Projectile.Move(target.Center, 16, 18);
            }
            else
                Projectile.velocity *= 1.03f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].velocity *= 3;
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].velocity *= 2;
                Main.dust[dust].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int g = 0; g < 2; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
        }
    }
}