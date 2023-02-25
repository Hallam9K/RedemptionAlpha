using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class CalciteWand_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calcite Stalagmites");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjEarth[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.1f;
            Projectile.frame = Main.rand.Next(3);
        }
        public override void AI()
        {
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.scale += 0.03f;
            Projectile.scale = MathHelper.Clamp(Projectile.scale, 0, 1);
            if (Projectile.scale >= 1)
            {
                Projectile.friendly = true;
                Projectile.velocity.Y += 0.3f;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
            for (int i = 0; i < 6; i++)
            {
                int dustIndex4 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
                Main.dust[dustIndex4].velocity *= 2f;
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, Scale: 0.5f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage += (int)Projectile.velocity.Y * 3;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage += (int)Projectile.velocity.Y * 3;
        }
    }
}