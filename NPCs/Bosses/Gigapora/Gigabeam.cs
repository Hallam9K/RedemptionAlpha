using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Base;
using Redemption.Globals;
using System.Collections.Generic;
using Redemption.BaseExtension;
using Redemption.Dusts;
using ReLogic.Content;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigabeam : LaserProjectile
    {
        private new readonly float FirstSegmentDrawDist = 460;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Gigabeam");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 350;
            Projectile.hide = true;
            LaserScale = 0.1f;
            LaserSegmentLength = 800;
            LaserWidth = 176;
            LaserEndSegmentLength = 200;
            MaxLaserLength = 2400;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(0.5f).UseColor(Color.Red).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            Main.LocalPlayer.ManageSpecialBiomeVisuals("MoR:FogOverlay", true);


            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < MaxLaserLength; i += 10)
            {
                if (Main.rand.NextBool(40))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * i, ModContent.DustType<GlowDust>(), Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(10, 20), 0, default, 2f);
                    dust.noGravity = true;
                    Color dustColor = new(216, 35, 10) { A = 0 };
                    dust.color = dustColor;
                }
                if (Main.rand.NextBool(40))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * i, ModContent.DustType<GlowDust>(), Vector2.UnitX.RotatedBy(Projectile.rotation) * Main.rand.NextFloat(10, 20), 0, default, 1f);
                    dust.noGravity = true;
                    Color dustColor = new(255, 200, 193) { A = 0 };
                    dust.color = dustColor;
                }
            }

            #region Beginning And End Effects
            if (AITimer == 0)
            {
                LaserScale = 0.1f;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(12, Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity);

            Projectile.Center = npc.Center + RedeHelper.PolarVector(14, npc.rotation - 1.57f);
            Projectile.velocity = RedeHelper.PolarVector(10, npc.rotation - 1.57f);

            if (AITimer <= 10)
                LaserScale += 0.06f;
            else if (Projectile.timeLeft < 10 || !npc.active || npc.ai[0] == 4)
            {
                Projectile.hostile = false;

                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;

                LaserScale -= 0.1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            #endregion

            LaserLength = MaxLaserLength;
            ++AITimer;
        }

        #region Drawcode
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.8f, 1.2f, 1.8f, 1.2f);
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 381) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 381) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.White), (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D glowRadius = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            float glowOpacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.3f, 0.5f, 0.3f);
            float glowSize = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 2.5f, 2f, 2.5f);
            float glowSize2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 2f, 2.5f, 2f);
            Vector2 glowOrigin = new(glowRadius.Width / 2f, glowRadius.Height / 2f);
            Main.EntitySpriteDraw(glowRadius, Projectile.Center - Main.screenPosition, null, Color.Red * glowOpacity, 0, glowOrigin, glowSize, 0, 0);
            Main.EntitySpriteDraw(glowRadius, Projectile.Center - Main.screenPosition, null, Color.Red * glowOpacity * 0.7f, 0, glowOrigin, glowSize2 * 1.5f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        #endregion

        #region Collisions
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;
            // Run an AABB versus Line check to look for collisions
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * (LaserLength - 20), 30 * LaserScale, ref point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}