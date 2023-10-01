using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.Effects.PrimitiveTrails;
using Redemption.Globals;
using Redemption.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class CurvingStar : ModProjectile, ITrailProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new RainbowTrail(5f, 0.002f, 1f, .75f), new RoundCap(), new DefaultTrailPosition(), 50f, 100f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.ai[1] == 0)
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 180) * Projectile.ai[0];
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / -180) * Projectile.ai[0];
        }
        public override void OnKill(int timeLeft)
        {
            ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new RainbowParticle(), Color.White, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Main.DiscoColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
    public class CurvingStar_Tele : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shooting Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 160;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha <= 170)
                    Projectile.localAI[0] = 1;
            }
            else
            {
                Projectile.alpha++;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[1] == 0)
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / 180) * Projectile.ai[0];
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.PI / -180) * Projectile.ai[0];
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }
    public class CurvingStar_Tele2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/CurvingStar_Tele";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shooting Star");
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 50;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CurvingStar_Tele>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0], Projectile.ai[1] == 0 ? 0 : 1);
                Projectile.localAI[0]++;
            }
            else
            {
                if (++Projectile.localAI[0] >= 30)
                {
                    for (int m = 0; m < 8; m++)
                    {
                        int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.Enchanted_Pink, 0f, 0f, 100, Color.White, 3f);
                        Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(4f, 0f), m / (float)8 * 6.28f);
                        Main.dust[dustID].noLight = false;
                        Main.dust[dustID].noGravity = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item117, Projectile.position);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CurvingStar>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.ai[0], Projectile.ai[1] == 0 ? 0 : 1);
                    Projectile.Kill();
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
    }
}