using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_Flame : ModProjectile
    {
        public override string Texture => "Redemption/Textures/FlameTexture";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flames");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 260;
        }

        public override void AI()
        {
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(4, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(0.2f).UseColor(Color.OrangeRed).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            Main.LocalPlayer.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);

            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0f);
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= 2)
                    Projectile.frame = 0;
            }
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<Gigapora_BodySegment>() || npc.ai[0] != 1)
                Projectile.Kill();

            Projectile.Center = npc.Center + RedeHelper.PolarVector(22 * Projectile.ai[1], npc.rotation) + RedeHelper.PolarVector(18, npc.rotation + MathHelper.PiOver2);
            for (int i = 0; i < 180; i += 10)
            {
                if (Main.rand.NextBool(40))
                {
                    ParticleManager.NewParticle(Projectile.Center + RedeHelper.PolarVector(i, npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0)), RedeHelper.PolarVector(Main.rand.Next(4, 19), npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0)), new EmberParticle(), Color.White, 1, 20, 1);
                }
                if (Main.rand.NextBool(200))
                {
                    ParticleManager.NewParticle(Projectile.Center + RedeHelper.PolarVector(i, npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0)), RedeHelper.PolarVector(Main.rand.Next(3, 8), npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0)), new EmberParticle(), Color.White, 1, 20);
                }
            }
            if (Projectile.timeLeft < 60)
            {
                Projectile.hostile = false;
                Projectile.alpha += 10;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0));
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 20,
                Projectile.Center + unit * 190, 60, ref point))
                return true;
            else
                return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.ai[0] > -1)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                int height = texture.Height / 3;
                int y = height * Projectile.frame;
                Rectangle rect = new(0, y, texture.Width, height);
                Vector2 drawOrigin = new(texture.Width / 2, height / 2);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0) + MathHelper.PiOver2, drawOrigin - new Vector2(0, 90), new Vector2(Projectile.scale, Projectile.scale * 1.5f), SpriteEffects.FlipVertically, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * 0.5f * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == 1 ? MathHelper.Pi : 0) + MathHelper.PiOver2, drawOrigin - new Vector2(0, 90), new Vector2(Projectile.scale + 0.2f, Projectile.scale * 1.5f + 0.2f), SpriteEffects.FlipVertically, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 200);
        }
    }
}