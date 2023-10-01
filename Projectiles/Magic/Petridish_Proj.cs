using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Petridish_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Petridish");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 40;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation += .06f;
            Projectile.velocity.Y += .4f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime);
                Main.dust[dustIndex].velocity *= 2;
            }
            for (int i = 0; i < 7; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass);
            if (Projectile.owner == Main.myPlayer)
            {
                int rand = Main.rand.Next(3, 5);
                for (int i = 0; i < rand; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.Spread(7), ModContent.ProjectileType<Bacteria_Proj>(), Projectile.damage, 0, Projectile.owner, Main.rand.Next(0, 3));
            }
        }
    }
}