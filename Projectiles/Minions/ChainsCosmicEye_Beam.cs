using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.Projectiles.Minions
{
    public class ChainsCosmicEye_Beam : LaserProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/CosmicEye_Beam";
        private new const float FirstSegmentDrawDist = 14;
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Ray");
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 40;
            LaserScale = 1;
            LaserSegmentLength = 28;
            LaserWidth = 26;
            LaserEndSegmentLength = 28;
            MaxLaserLength = 800;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            Projectile eye = Main.projectile[(int)Projectile.ai[0]];
            if (AITimer == 0)
                LaserScale = 0.1f;

            if (eye.active)
            {
                Projectile.Center = eye.Center;
                Projectile.velocity = RedeHelper.PolarVector(10, eye.rotation + (float)-Math.PI / 2);
            }

            if (AITimer <= 10)
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

            float dist = Main.npc[(int)eye.ai[0]].Distance(eye.Center);
            LaserLength = Math.Min(dist, MaxLaserLength);
            ++AITimer;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 8;
            target.immune[Projectile.owner] = 0;
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

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(RedeColor.NebColour), (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        #endregion
    }
}