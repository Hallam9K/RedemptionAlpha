using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolyBeam : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Erhan/ScorchingRay";

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
        public float LaserScale = 0.3f;
        public int LaserSegmentLength = 60;
        public int LaserWidth = 76;
        public int LaserEndSegmentLength = 60;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 30;

        public int MaxLaserLength = 1800;
        public int maxLaserFrames = 1;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = false;
        // >
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Beam");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target) => AITimer >= 80;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 80 ? null : false;
        public override bool ShouldUpdatePosition() => false;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver2;
            #region Beginning And End Effects
            if (AITimer == 80 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Bass1, Projectile.position);

            if (AITimer >= 80)
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 200, 255);
            }
            if (Projectile.timeLeft < 10)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }
                Projectile.alpha += 20;
            }
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

            #region Frame and Timer Updates
            /*++Projectile.frameCounter;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                {
                    Frame = 0;
                }
            }*/
            ++AITimer;
            #endregion

            #region misc
            //CutTiles();
            if (AITimer >= 80)
                CastLights();
            #endregion
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
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
        private void CastLights()
        {
            // Cast a light along the line of the Laser
            DelegateMethods.v3_1 = new Vector3(1f, 0.7f, 0f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserLength, 26, DelegateMethods.CastLight);
        }
        #endregion

        #region Drawcode
        // The core function of drawing a Laser, you shouldn't need to touch this
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {

            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.8f, 1.2f, 1.8f, 1.2f);
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(256 * Frame), LaserEndSegmentLength, 256, LaserSegmentLength), color, r,
                    new Vector2(256 / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(256 * Frame), LaserEndSegmentLength, 256, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(256 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), 0, 256, LaserEndSegmentLength), color, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), LaserSegmentLength + LaserEndSegmentLength, 256, LaserEndSegmentLength), color, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), 0, 256, LaserEndSegmentLength), color * 0.5f, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), LaserSegmentLength + LaserEndSegmentLength, 256, LaserEndSegmentLength), color * 0.5f, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.8f, 1f, 0.8f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.White) * opacity, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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
                Projectile.Center + unit * LaserLength, 48 * LaserScale, ref point))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region MP Sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaserLength);
            writer.Write(LaserScale);
            writer.Write(LaserSegmentLength);
            writer.Write(LaserEndSegmentLength);
            writer.Write(LaserWidth);
            writer.Write(MaxLaserLength);
            //writer.Write(maxLaserFrames);
            //writer.Write(LaserFrameDelay);
            writer.Write(StopsOnTiles);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            LaserLength = reader.ReadSingle();
            LaserScale = reader.ReadSingle();
            LaserSegmentLength = reader.ReadInt32();
            LaserEndSegmentLength = reader.ReadInt32();
            LaserWidth = reader.ReadInt32();
            MaxLaserLength = reader.ReadInt32();
            StopsOnTiles = reader.ReadBoolean();
            //maxLaserFrames = reader.
        }
        #endregion
    }
}