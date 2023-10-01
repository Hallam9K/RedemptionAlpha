using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_CrossBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cross Bomb");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 600;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 2)
                    Projectile.frame = 0;
            }
            Projectile.velocity *= .9f;
            if (Projectile.velocity.Length() < 4)
            {
                Projectile.rotation.SlowRotation(0, MathHelper.Pi / 80);
                Projectile.localAI[0]++;
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.localAI[0] == 20)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ShieldActivate, Projectile.position);
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Gigapora_CrossBombTele>(), 0, 0, Main.myPlayer, Projectile.whoAmI, i);
                        }
                    }
                    if (Projectile.localAI[0] == 80)
                    {
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                        Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.PlasmaShot, Projectile.position);
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(2, Projectile.rotation + (MathHelper.PiOver2 * i)), ModContent.ProjectileType<CrossBomb_Beam>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                        }
                    }
                }
                if (Projectile.localAI[0] >= 80)
                {
                    Projectile.alpha += 10;
                    if (Projectile.alpha >= 255)
                        Projectile.Kill();
                }
            }
            else
                Projectile.rotation += Projectile.velocity.X / 40 * Projectile.direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, 0, 0);
            return false;
        }
    }
    public class Gigapora_CrossBombTele : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.alpha = 240;
        }
        public override void AI()
        {
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            if (!proj.active || proj.type != ModContent.ProjectileType<Gigapora_CrossBomb>())
                Projectile.Kill();

            Projectile.Center = proj.Center;
            Projectile.rotation = MathHelper.PiOver2 * Projectile.ai[1];
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                return;
            }
            Projectile.localAI[0] += 15;
            Projectile.alpha -= 4;
            if (Projectile.alpha <= 0)
                Projectile.localAI[1] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.DarkRed * Projectile.Opacity, Projectile.rotation, new Vector2(0, 64), new Vector2(Projectile.localAI[0] / 60f, Projectile.width / 128f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.DarkRed * Projectile.Opacity, -Projectile.rotation, new Vector2(0, 64), new Vector2(Projectile.width / 128f, Projectile.width / 128f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}