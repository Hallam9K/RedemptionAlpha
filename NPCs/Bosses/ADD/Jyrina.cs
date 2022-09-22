using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class Jyrina : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jyrina");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 94;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 400;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 9)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 9;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, (SpriteEffects)Projectile.ai[0], 0);
            Main.EntitySpriteDraw(glowMask, position, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, (SpriteEffects)Projectile.ai[0], 0);

            return false;
        }
    }
}