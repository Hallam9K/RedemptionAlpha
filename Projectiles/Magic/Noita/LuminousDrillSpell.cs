using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class LuminousDrillSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Luminous Drill");
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 40;
            Projectile.extraUpdates = 8;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.alpha = 255;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);

            if (Main.myPlayer == Projectile.owner)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                    vector13 = Vector2.UnitX * player.direction;
                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = vector13;
                if (player.noItems || player.CCed || player.dead || !player.active)
                {
                    Projectile.Kill();
                }
                Projectile.netUpdate = true;
            }
            Projectile.Center = player.MountedCenter + RedeHelper.PolarVector(80, Projectile.velocity.ToRotation());
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.alpha = Main.rand.Next(0, 255);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity * 2;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 26,
                Projectile.Center + unit * 26, 4, ref point))
                return true;
            return false;
        }
    }
}