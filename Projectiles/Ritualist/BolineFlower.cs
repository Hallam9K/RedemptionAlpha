using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ritualist
{
    public class BolineFlower : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Boline Flower");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 800;
            Projectile.scale = 0.1f;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.LookByVelocity();
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.6f, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.4f);
            Projectile.velocity *= 0.96f;
            Projectile.rotation += Projectile.velocity.Length() / 40 * Projectile.spriteDirection;
            if (Projectile.scale >= 1 && Projectile.alpha <= 0 && Projectile.velocity.Length() < 0.1f)
            {
                if (Projectile.localAI[0]++ % 40 == 0)
                    RedeDraw.SpawnCirclePulse(Projectile.Center, Color.RosyBrown, 0.9f);

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) > 300 * 300)
                        continue;

                    player.AddBuff(ModContent.BuffType<BolineFlowerBuff>(), 4);
                }
            }

            if (Projectile.scale < 1)
            {
                Projectile.alpha -= 30;
                Projectile.scale += 0.1f;
            }
            else if (Projectile.timeLeft <= 60)
                Projectile.alpha = (int)MathHelper.Lerp(255f, 0f, Projectile.timeLeft / 60f);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }
            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
}