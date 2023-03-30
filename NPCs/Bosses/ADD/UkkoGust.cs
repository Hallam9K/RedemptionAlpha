using System;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoGust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gust");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.alpha = 254;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 180) * 1.02f;
            if (Projectile.localAI[0] == 0)
                Projectile.alpha -= 4;
            else
                Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 60)
                Projectile.alpha += 2;
            if (Projectile.alpha < 180)
                Projectile.localAI[0] = 1;

            if (Math.Abs(Projectile.velocity.X) > 0.2)
                Projectile.spriteDirection = -Projectile.direction;

            for (int p = 0; p < 255; p++)
            {
                Player player = Main.player[p];
                if (player.active && !player.dead && Projectile.alpha < 250 && Projectile.Hitbox.Intersects(player.Hitbox))
                    player.velocity = Projectile.velocity / 2;
            }
            if ((Projectile.alpha >= 255 && Projectile.localAI[0] != 0) || (Projectile.velocity.X == 0 && Projectile.velocity.Y == 0))
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(Projectile.width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            }
            return true;
        }
    }
}