using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Redemption.Globals;
using System;
using Terraria.Audio;

namespace Redemption.Projectiles.Magic
{
    public class MagnifyingGlassRay : LaserProjectile
    {
        private new const float FirstSegmentDrawDist = 10;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Scorching Ray");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            LaserScale = 0.5f;
            LaserSegmentLength = 20;
            LaserWidth = 17;
            LaserEndSegmentLength = 20;
            MaxLaserLength = 1200;
        }

        public override bool CanHitPlayer(Player target) => AITimer >= 80;
        public override bool? CanHitNPC(NPC target) => !target.friendly && AITimer >= 80 ? null : false;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + new Vector2(20 * player.direction, -22);
            Projectile.velocity = RedeHelper.PolarVector(1, player.direction == -1 ? (float)Math.PI - 0.3f : 0.3f);

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;


            if (AITimer >= 80 && AITimer % 20 == 0)
            {
                int mana = player.inventory[player.selectedItem].mana;
                if (BasePlayer.ReduceMana(player, mana / 2))
                {
                    if (player.channel)
                        Projectile.timeLeft = 200;
                }
                else
                {
                    player.channel = false;
                }
            }
            #region Beginning And End Effects
            if (AITimer == 80)
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
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
            if (Projectile.timeLeft < 10 || !player.channel || !player.active || player.dead)
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
                    new Rectangle((int)(64 * Frame), LaserEndSegmentLength, 64, LaserSegmentLength), color, r,
                    new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(64 * Frame), LaserEndSegmentLength, 64, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(64 * Frame), 0, 64, LaserEndSegmentLength), color, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 5) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(64 * Frame), LaserSegmentLength + LaserEndSegmentLength, 64, LaserEndSegmentLength), color, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(64 * Frame), 0, 64, LaserEndSegmentLength), color * 0.5f, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 5) * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(64 * Frame), LaserSegmentLength + LaserEndSegmentLength, 64, LaserEndSegmentLength), color * 0.5f, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.8f, 1f, 0.8f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength,
                Projectile.GetAlpha(Main.dayTime ? Color.White : Color.CornflowerBlue) * opacity, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        #endregion
    }
}