using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.ADD
{
    public class UkkoStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ukko's Lightning");
            Main.projFrames[Projectile.type] = 24;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 540;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale *= 2;
            Projectile.alpha = 255;
        }
        public int warningFrames;
        public int frameCounters;
        public override void AI()
        {
            frameCounters++;
            if (frameCounters >= 3)
            {
                warningFrames++;
                frameCounters = 0;
            }
            if (warningFrames >= 2)
                warningFrames = 0;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                    Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity, Projectile.Opacity, Projectile.Opacity);
            Projectile.localAI[0]++;
            if (Projectile.frame >= 12 && Projectile.frame < 15)
                Projectile.hostile = true;
            else
                Projectile.hostile = false;

            if (Projectile.localAI[0] == 1)
            {
                Projectile.position.Y -= 540;
                Projectile.alpha = 0;
            }
            if (Projectile.localAI[0] == 36)
            {
                Player player = Main.player[Projectile.owner];
                Main.NewLightning();
                player.GetModPlayer<ScreenPlayer>().Rumble(10, 10);
                SoundEngine.PlaySound(CustomSounds.Thunderstrike, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Bottom.Y), Projectile.velocity, ModContent.ProjectileType<UkkoStrikeZap>(), (int)(Projectile.damage * 1.2f), Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D warning = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/ADD/LightningWarning").Value;
            int height = texture.Height / 24;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, 0, 0);

            int height2 = warning.Height / 2;
            int y2 = height2 * warningFrames;
            Rectangle rect2 = new(0, y2, warning.Width, height2);
            Vector2 origin2 = new(warning.Width / 2f, height2 / 2f);

            if (Projectile.frame < 12)
                Main.EntitySpriteDraw(warning, position, new Rectangle?(rect2), Projectile.GetAlpha(Color.White) * 0.8f, Projectile.rotation + MathHelper.PiOver2, origin2, Projectile.scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
    public class UkkoStrikeZap : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ukko's Lightning");
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Scale: 1.2f);
                    Main.dust[dustIndex].velocity *= 2;
                }
                Projectile.localAI[0] = 1;
            }
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Electrified, target.HasBuff(BuffID.Wet) ? 320 : 160);
        }
    }
}