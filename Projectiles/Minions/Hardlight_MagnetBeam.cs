using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.Audio;
using Terraria.ID;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Minions
{
    public class Hardlight_MagnetBeam : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/KSIII/KS3_MagnetBeam";

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
        public float LaserScale = 0;
        public int LaserSegmentLength = 30;
        public int LaserWidth = 42;
        public int LaserEndSegmentLength = 40;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 20;

        public int MaxLaserLength = 1800;
        public int maxLaserFrames = 1;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = false;
        // >
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magnet Beam");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.Redemption().ParryBlacklist = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            if (AITimer == 0)
            {
                if (Projectile.damage >= 10 * 8)
                {
                    float loudness = Projectile.damage / (15 * 8);
                    loudness = MathHelper.Clamp(loudness, 1, 2);
                    SoundEngine.PlaySound(SoundID.Zombie104 with { Volume = loudness }, Projectile.position);
                }
                LaserScale = 0.2f;
            }

            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            if (!proj.active || proj.type != ModContent.ProjectileType<Hardlight_Magnet>())
                Projectile.Kill();

            Projectile.Center = proj.Center;

            if (AITimer < Projectile.damage / (2 * 8))
                LaserScale += 0.09f;

            if (Projectile.damage > 5 * 8)
            {
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, Projectile.damage / (5 * 8));
            }
            if (Projectile.timeLeft < 30 || !proj.active)
            {
                Projectile.alpha += 4;
            }
            else
                LaserScale = MathHelper.Clamp(LaserScale, 0.2f, 6);
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
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.White), (int)FirstSegmentDrawDist);

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