using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class GiantMask : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Giant Mask");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjShadow[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 138;
            Projectile.height = 184;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 720;
        }

        public Vector2[] maskPos = new Vector2[5];
        public int[] maskFrame = new int[5];
        public int maskAlpha = 255;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.noItems || player.CCed || player.dead || !player.active)            
                Projectile.Kill();
            
            switch (Projectile.ai[1])
            {
                case 0:
                    Projectile.scale = 0.01f;
                    Projectile.ai[1] = 1;
                    break;
                case 1:
                    maskAlpha -= 5;
                    Projectile.localAI[0] += 1;
                    Projectile.localAI[0] *= 1.02f;
                    if (Projectile.localAI[1] < 100) { Projectile.localAI[1] += 1; }
                    for (int i = 0; i < 5; i++)
                    {
                        maskPos[i] = Projectile.Center + Vector2.One.RotatedBy(MathHelper.ToRadians((360 / 5 * i) + Projectile.localAI[0])) * Projectile.localAI[1];
                    }
                    if (maskAlpha <= 0)
                    {
                        Projectile.alpha -= 4;
                        if (Projectile.scale < 1) { Projectile.scale += 0.05f; }
                    }
                    if (Projectile.alpha < 0 && Projectile.scale >= 1)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int m = 0; m < 16; m++)
                            {
                                int dustID = Dust.NewDust(new Vector2(maskPos[i].X - 1, maskPos[i].Y - 1), 2, 2, DustID.AncientLight, 0f, 0f, 100, Color.White, 1);
                                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(3f, 0f), m / (float)16 * 6.28f);
                                Main.dust[dustID].noLight = false;
                                Main.dust[dustID].noGravity = true;
                            }
                        }
                        maskAlpha = 255;
                        Projectile.alpha = 0;
                        Projectile.scale = 1;
                        Projectile.ai[1] = 2;
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 6;
                    }
                    break;
                case 2:
                    Projectile.friendly = true;
                    if (++Projectile.frameCounter >= 15)
                    {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 2)
                        {
                            Projectile.frame = 0;
                        }
                    }                 
                    break;
            }
            if (Projectile.timeLeft < 60)
            {
                Projectile.hostile = false;
                Projectile.alpha += 5;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, lightColor, Color.GhostWhite, lightColor);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D masks = ModContent.Request<Texture2D>("Redemption/Projectiles/Magic/MaskFacesFront").Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k];
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(texture, drawPos + Projectile.Size / 2f - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 0; i < 5; i++)
            {
                int height2 = masks.Height / 5;
                Vector2 origin2 = new(masks.Width / 2, height2 / 2);
                maskFrame[i] = i;
                Main.spriteBatch.Draw(masks, maskPos[i] - Main.screenPosition, new Rectangle?(new Rectangle(0, height2 * maskFrame[i], masks.Width, height2)), lightColor * ((255 - maskAlpha) / 255f), Projectile.rotation, origin2, 1, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}