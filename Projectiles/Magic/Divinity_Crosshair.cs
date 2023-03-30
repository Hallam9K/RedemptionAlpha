using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Weapons.HM.Magic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Divinity_Crosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crosshair");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.timeLeft = 4;
            Projectile.rotation += 0.05f;
            Player player = Main.player[Projectile.owner];
            Projectile sun = Main.projectile[(int)Projectile.ai[0]];
            if (!player.active || player.dead || !sun.active || sun.type != ModContent.ProjectileType<Divinity_Sun>())
                Projectile.Kill();

            if (Projectile.owner == Main.myPlayer && player.channel)
                Projectile.Center = Main.MouseWorld;
            else
            {
                if (sun.ModProjectile is Divinity_Sun sunEntity && sunEntity.mark != Vector2.Zero)
                    Projectile.Center = sunEntity.mark;
            }
            Projectile.alpha -= 10;
            Projectile.alpha = (int)MathHelper.Max(Projectile.alpha, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.5f, -Projectile.rotation, drawOrigin, Projectile.scale * 1.5f, SpriteEffects.None, 0);
            return false;
        }
    }
}