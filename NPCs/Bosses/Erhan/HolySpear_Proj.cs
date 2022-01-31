using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolySpear_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.Redemption().Unparryable = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.timeLeft <= 540)
                Projectile.velocity *= 1.05f;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.position += Projectile.velocity * (Projectile.ai[0] == 1 ? 2000 : 1200);
                DustHelper.DrawCircle(Projectile.Center, DustID.GoldFlame, 2, 2, 2, 1, 4, nogravity: true);
                Projectile.alpha = 0;
                Projectile.localAI[0] = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = new Color(255, 255, 120) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale + 0.2f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
