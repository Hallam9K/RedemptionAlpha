using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Laser : ModProjectile
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
        public int LaserWidth = 40;
        public int LaserEndSegmentLength = 46;
        
        //should be set to about half of the end length
        private readonly float FirstSegmentDrawDist = 23;

        public int MaxLaserLength = 1840;
        public int maxLaserFrames = 3;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = true;
        // >
        public override void SetStaticDefaults() => DisplayName.SetDefault("Xenium Laser");
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.alpha = 255;
            Projectile.Redemption().ParryBlacklist = true;
        }
        public override bool CanHitPlayer(Player target) => AITimer >= 85;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 85 ? null : false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch);
            Main.dust[dust].velocity = RedeHelper.PolarVector(6, Projectile.rotation);
            Main.dust[dust].noGravity = true;

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 0.1f;

            if (AITimer >= 85)
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 2);
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 180, 255);
            }
            if (AITimer <= 10)
                LaserScale += 0.09f;
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            if (npc.active && npc.type == ModContent.NPCType<PZ>())
            {
                Projectile.Center = npc.Center;
                switch (Projectile.ai[1])
                {
                    case 0:
                        Projectile.velocity = RedeHelper.PolarVector(10, npc.rotation);
                        break;
                    case 1:
                        Projectile.velocity = RedeHelper.PolarVector(10, npc.rotation + MathHelper.Pi);
                        break;
                    case 2:
                        Projectile.velocity = RedeHelper.PolarVector(10, npc.rotation + MathHelper.PiOver2);
                        break;
                    case 3:
                        Projectile.velocity = RedeHelper.PolarVector(10, npc.rotation - MathHelper.PiOver2);
                        break;
                    case 4:
                        Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
                        break;
                    case 5:
                        Projectile.velocity = Projectile.velocity.RotatedBy(-0.01f);
                        break;
                    case 6:
                        Projectile.velocity = RedeHelper.PolarVector(10, -npc.rotation);
                        break;
                    case 7:
                        Projectile.velocity = RedeHelper.PolarVector(10, -npc.rotation + MathHelper.Pi);
                        break;
                    case 8:
                        Projectile.velocity = RedeHelper.PolarVector(10, -npc.rotation + MathHelper.PiOver2);
                        break;
                    case 9:
                        Projectile.velocity = RedeHelper.PolarVector(10, -npc.rotation - MathHelper.PiOver2);
                        break;
                }
            }

            if (Projectile.timeLeft < 10 || !npc.active || npc.ai[0] != 1)
            {
                Projectile.hostile = false;
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;

                LaserScale -= 0.1f;
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
                Rectangle box = new((int)(RedeGen.LabVector.X + 109) * 16, (int)(RedeGen.LabVector.Y + 170) * 16, 69 * 16, 41 * 16);
                if (!box.Intersects(new Rectangle((int)start.X, (int)start.Y, 1, 1)))
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
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(AITimer >= 85 ? Color.White : Color.Red), (int)FirstSegmentDrawDist);

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

        #region MP Sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaserLength);
            writer.Write(LaserScale);
            writer.Write(LaserSegmentLength);
            writer.Write(LaserEndSegmentLength);
            writer.Write(LaserWidth);
            writer.Write(MaxLaserLength);
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
        }
        #endregion
    }
}