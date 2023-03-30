using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Redemption.Globals;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PiercingNebulaWeapon_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sword of the Forgotten");
            Main.projFrames[Projectile.type] = 6;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        private Vector2 rand;
        public override void AI()
        {
            if (Projectile.ai[0]++ == 0)
            {
                rand = RedeHelper.Spread(100);
            }
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.Center = player.MountedCenter + rand;
            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            if (Projectile.frameCounter++ % 3 == 0)
            {
                if (++Projectile.frame > 5 && Projectile.owner == Main.myPlayer)
                {
                    Vector2 pos = Projectile.Center + RedeHelper.PolarVector(20 * Projectile.spriteDirection, (player.Center - Main.MouseWorld).ToRotation() + MathHelper.PiOver2);
                    SoundEngine.PlaySound(SoundID.Item125, player.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, RedeHelper.PolarVector(player.inventory[player.selectedItem].shootSpeed, (Main.MouseWorld - pos).ToRotation()), ModContent.ProjectileType<PNebula1_Friendly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[0] > 1)
                Projectile.alpha -= 30;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}