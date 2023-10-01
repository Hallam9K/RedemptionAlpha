using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly
{
    public class KS3Sitting_Hologram : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Friendly/Hologram";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hologram");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.timeLeft = 380;
        }
        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= 300)
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 4)
                        Projectile.frame = 2;
                }
            }
            else
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= 7)
                        Projectile.Kill();
                }
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.3f, Projectile.Opacity * 0.6f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            switch (Projectile.ai[0])
            {
                case 1:
                    texture = ModContent.Request<Texture2D>(Texture + "_Lab").Value;
                    break;
                case 2:
                    texture = ModContent.Request<Texture2D>(Texture + "_Planet").Value;
                    break;
                case 3:
                    if (RedeWorld.slayerRep >= 3)
                        texture = ModContent.Request<Texture2D>(Texture + "_Ship2").Value;
                    else
                        texture = ModContent.Request<Texture2D>(Texture + "_Ship").Value;
                    break;
            }
            int height = texture.Height / 7;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}