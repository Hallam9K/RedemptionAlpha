using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Terraria.Audio;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class ScorchingRay : LaserProjectile
    {
        private new const float FirstSegmentDrawDist = 30;
        // >
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Scorching Ray");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
            LaserScale = 1;
            LaserSegmentLength = 60;
            LaserWidth = 76;
            LaserEndSegmentLength = 60;
            MaxLaserLength = 1800;
            StopsOnTiles = false;
        }

        public override bool CanHitPlayer(Player target) => AITimer >= 80;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 80 ? null : false;
        public override bool ShouldUpdatePosition() => false;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
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
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 150, 255);
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

            ++AITimer;

            if (AITimer >= 80)
                CastLights(new Vector3(1f, 0.7f, 0f));
        }

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
            Main.EntitySpriteDraw(texture, start + (maxDist + 30) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), LaserSegmentLength + LaserEndSegmentLength, 256, LaserEndSegmentLength), color, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(256 * Frame), 0, 256, LaserEndSegmentLength), color * 0.5f, r, new Vector2(256 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 30) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
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
    }
}