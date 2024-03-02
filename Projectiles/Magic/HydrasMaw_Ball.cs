using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class HydrasMaw_Ball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjWater[Type] = true;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 1f, 1f);
            Projectile.rotation += Projectile.velocity.X / 40;
            Projectile.velocity.Y += .2f;
            Projectile.velocity.X *= .99f;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || !proj.friendly || proj.type != ModContent.ProjectileType<HydrasMaw_Proj>())
                    continue;

                if (!Projectile.Hitbox.Intersects(proj.Hitbox))
                    continue;

                Projectile.localAI[0] = 1;
                Projectile.timeLeft = 2;
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item107, Projectile.position);
            for (int i = 0; i < 25; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HydraAcidDust>(), 0f, 0f, 100, default, 2f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, Scale: 1f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Projectile.localAI[0] is 1)
                {
                    for (int i = 0; i < 6; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-4, 7), Main.rand.Next(-6, -3)), ModContent.ProjectileType<HydrasMaw_Proj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

                    for (int i = 0; i < 18; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(8, 12), MathHelper.ToRadians(20) * i), ModContent.ProjectileType<HydrasMaw_Gas>(), Projectile.damage, 0, Projectile.owner);
                }
                for (int i = 0; i < 12; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(6, 8), MathHelper.ToRadians(30) * i), ModContent.ProjectileType<HydrasMaw_Gas>(), Projectile.damage, 0, Projectile.owner);
                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, RedeHelper.PolarVector(Main.rand.Next(3, 4), MathHelper.ToRadians(60) * i), ModContent.ProjectileType<HydrasMaw_Gas>(), Projectile.damage, 0, Projectile.owner);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
    }
}