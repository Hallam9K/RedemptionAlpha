using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class LunarShot_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunar Bolt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (!Main.dayTime && Main.moonPhase != 4 && Main.myPlayer == player.whoAmI) {
                if (Main.moonPhase == 0) 
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(8, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<MoonflareBatIllusion>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack, Main.myPlayer, target.whoAmI);

                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(8, Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<MoonflareBatIllusion>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack, Main.myPlayer, target.whoAmI);
            }
        }
        public override void Kill(int timeLeft)
        {
            DustHelper.DrawCircle(Projectile.Center, ModContent.DustType<MoonflareDust>(), 1, 2, 2, nogravity: true);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity *= 2;
                Projectile.localAI[0] = 1;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            lightColor.A = 0;
            return lightColor;
        }
    }
}