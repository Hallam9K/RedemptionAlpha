using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Redemption.Globals;
using Redemption.BaseExtension;
using Redemption.Dusts;

namespace Redemption.NPCs.Lab.Volt
{
    public class TeslaBeam : ModProjectile
    {
        public float AITimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float Frame
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public float LaserLength = 0;
        public float LaserScale = 0.1f;
        public int LaserSegmentLength = 46;
        public int LaserWidth = 46;
        public int LaserEndSegmentLength = 46;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 23;

        public int MaxLaserLength = 1840;
        public int maxLaserFrames = 3;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = true;
        // >
        public override void SetStaticDefaults()
        {
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.Redemption().ParryBlacklist = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            Main.dust[dust].velocity = RedeHelper.PolarVector(6, Projectile.rotation);
            Main.dust[dust].noGravity = true;

            #region Beginning And End Effects
            if (AITimer == 0)
            {
                LaserScale = 0.1f;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            if (npc.active && npc.type == ModContent.NPCType<ProtectorVolt>())
            {
                float gunRot = (npc.ModNPC as ProtectorVolt).gunRot;
                Projectile.Center = npc.Center + RedeHelper.PolarVector(60, gunRot) + RedeHelper.PolarVector(-4 * npc.spriteDirection, gunRot - (float)Math.PI / 2);
                Projectile.velocity = RedeHelper.PolarVector(10, (npc.ModNPC as ProtectorVolt).gunRot);
            }

            if (AITimer <= 10)
                LaserScale += 0.09f;
            else if (Projectile.timeLeft < 10 || !npc.active || npc.ai[0] == 1 || npc.ai[2] == 2)
            {
                Projectile.hostile = false;

                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;

                LaserScale -= 0.1f;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    int num5 = Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength + 30) - new Vector2(5, 5), 10, 10, ModContent.DustType<GlowDust>(), 0, 0, Scale: 2);
                    Color dustColor = new(193, 255, 219) { A = 0 };
                    Main.dust[num5].velocity *= 0;
                    Main.dust[num5].color = dustColor * Projectile.Opacity;
                    Main.dust[num5].noGravity = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            #endregion

            #region Length Setting
            if (StopsOnTiles)
            {
                EndpointTileCollision();
            }
            else
            {
                LaserLength = MaxLaserLength;
            }
            #endregion

            ++Projectile.frameCounter;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                    Frame = 0;
            }
            ++AITimer;
        }

        #region Laser AI Submethods
        private void EndpointTileCollision()
        {
            for (LaserLength = FirstSegmentDrawDist; LaserLength < MaxLaserLength; LaserLength += LaserSegmentLength)
            {
                Vector2 start = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, start, 1, 1))
                {
                    LaserLength -= LaserSegmentLength;
                    break;
                }
            }
        }
        #endregion

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
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
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