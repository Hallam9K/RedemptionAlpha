using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.NPCs.Bosses.Neb.Phase2;

namespace Redemption.NPCs.Bosses.Neb
{
    public class CosmicEye_Beam : ModProjectile
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
        public float LaserScale = 1;
        public int LaserSegmentLength = 28;
        public int LaserWidth = 26;
        public int LaserEndSegmentLength = 28;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 14;

        public int MaxLaserLength = 2000;
        // >
        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Ray");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 140;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            Projectile eye = Main.projectile[(int)Projectile.ai[0]];
            if (AITimer == 0)
            {
                if (eye.type == ModContent.ProjectileType<CosmicEye3>())
                    Projectile.timeLeft = 80;
                LaserScale = 0.1f;
            }

            if (AITimer == 20)
                SoundEngine.PlaySound(SoundID.Item125, Projectile.position);

            if (eye.active && eye.type == ModContent.ProjectileType<CosmicEye>())
            {
                Projectile.Center = eye.Center;
                Projectile.velocity = RedeHelper.PolarVector(10, eye.rotation + (float)-Math.PI / 2);
            }

            if (AITimer > 20 && AITimer <= 30)
                LaserScale += 0.09f;
            else if (Projectile.timeLeft < 10 || !eye.active)
            {
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
        public override bool CanHitPlayer(Player target)
        {
            return AITimer > 20;
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
                Projectile.Center + unit * LaserLength, Projectile.width * LaserScale, ref point))
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