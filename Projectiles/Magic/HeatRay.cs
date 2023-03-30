using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.ID;
using Redemption.Globals;
using System;
using Redemption.Buffs.NPCBuffs;
using Redemption.BaseExtension;
using Redemption.Particles;
using ParticleLibrary;

namespace Redemption.Projectiles.Magic
{
    public class HeatRay : LaserProjectile
    {
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Ray");
            ElementID.ProjFire[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire3, 60);

            if (player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            Projectile.rotation = host.rotation + (host.spriteDirection == -1 ? (float)Math.PI : 0);

            ParticleManager.NewParticle(Projectile.position + new Vector2(Main.rand.Next(0, Projectile.width), Main.rand.Next(0, Projectile.height)), RedeHelper.PolarVector(5, Projectile.rotation), new EmberParticle(), Color.White, 1);

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 0.1f;
            else
                Projectile.Center = host.Center + RedeHelper.PolarVector(6, host.rotation + MathHelper.PiOver2);

            if (AITimer <= 10)
            {
                LaserScale += 0.18f;
            }
            else if (!player.channel || Projectile.timeLeft < 10 || !player.active)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }
                LaserScale -= 0.1f;
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
        }
        #region Drawcode
        // The core function of drawing a Laser, you shouldn't need to touch this
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                //Color c = Color.White;


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

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Color.White, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        #endregion
    }
}