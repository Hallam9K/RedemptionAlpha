using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class FanOShivs_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shiv");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Iron, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.1f;
            if (Projectile.timeLeft < 560)
            {
                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y += 1f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
    }
    public class FanOShivsPoison_Proj : FanOShivs_Proj
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Poisoned Shiv");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override bool PreAI()
        {
            if (Main.rand.NextBool(20))
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Poisoned, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 600);
        }
    }
}