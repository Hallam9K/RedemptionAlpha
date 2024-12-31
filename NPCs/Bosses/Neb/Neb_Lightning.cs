using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using Redemption.Textures;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Neb_Lightning_Tele : LaserProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/CosmicEye_Beam";
        private new const float FirstSegmentDrawDist = 14;
        public override bool ShouldUpdatePosition() => true;
        public override void SetSafeDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 140;
            Projectile.alpha = 255;
            LaserScale = 1.5f;
            LaserSegmentLength = 28;
            LaserWidth = 26;
            LaserEndSegmentLength = 28;
            MaxLaserLength = 3000;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects

            Projectile.velocity.X *= .98f;

            if (AITimer < 30)
                Projectile.alpha -= 2;
            if (AITimer > 90)
            {
                Main.LocalPlayer.GetModPlayer<ScreenPlayer>().Rumble(10, 16);
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Thunderstrike, Projectile.position);
                Main.NewLightning();
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 5), ProjectileType<Neb_Lightning>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.Kill();
            }
            else
                Projectile.alpha--;

            Projectile.rotation = MathHelper.PiOver2;

            #endregion

            LaserLength = MaxLaserLength;
            ++AITimer;
        }
        #region Drawcode
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(new(193, 255, 219)), (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
    }
    public class Neb_Lightning : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning");
            ElementID.ProjThunder[Type] = true;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 80;
            Projectile.timeLeft = 1000;
            timeLeftMax = Projectile.timeLeft;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        int timeLeftMax;
        public Color baseColor = new(193, 255, 219, 0);
        public Color endColor = new(0, 242, 170, 0);
        public Color edgeColor = Color.White with { A = 0 };
        private DanTrail trail;
        private DanTrail trail2;
        private Vector2 originalVector;
        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                originalVector = Projectile.velocity;
                Projectile.ai[1] = 1;
            }
            Color bright = new(255, 255, 255, 0);
            Color mid = new(161, 255, 253, 0);
            Color dark = new(40, 186, 242, 0);
            Color emberColor = Color.Multiply(Color.Lerp(bright, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1f);
            Color glowColor = Color.Multiply(Color.Lerp(mid, dark, (float)(timeLeftMax - Projectile.timeLeft) / timeLeftMax), 1f);

            RedeParticleSystemManager.RedeQuadSystem.NewParticle(Projectile.Center, Vector2.Zero, new QuadParticle()
            {
                StartColor = emberColor,
                EndColor = glowColor,
                Scale = new Vector2(.4f),
            }, 10);
            if (Projectile.ai[0]++ % 5 == 0)
            {
                Projectile.velocity = originalVector.RotatedByRandom(Main.rand.NextFloat(-1f, 1f));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(CommonTextures.WhiteOrb.Width() / 2, CommonTextures.WhiteOrb.Height() / 2);
            Main.EntitySpriteDraw(CommonTextures.WhiteOrb.Value, Projectile.Center - (Projectile.velocity / 20) - Main.screenPosition, null, new(193, 255, 219, 0), Projectile.rotation, drawOrigin, .7f * Projectile.Opacity, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(CommonTextures.WhiteOrb.Value, Projectile.Center - (Projectile.velocity / 20) - Main.screenPosition, null, new(193, 255, 219, 0), Projectile.rotation, drawOrigin, .7f * Projectile.Opacity, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }
    }
}
