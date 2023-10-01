using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class BileLauncher_Gloop : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Hostile/OozeBall_Proj";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Radioactive Bile");
            Main.projFrames[Projectile.type] = 4;
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
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 160;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 1;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity.Y += .2f;
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble, Alpha: 100, Scale: 1);
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ToxicGas_Proj>(), Projectile.damage, 0, Projectile.owner, 1);

            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ToxicBubble);
        }
    }
}