using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            LaserScale = 0.5f;
            LaserSegmentLength = 20;
            LaserWidth = 17;
            LaserEndSegmentLength = 14;
            MaxLaserLength = 1200;

            NewCollision = true;
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
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.Center = playerCenter + new Vector2(20 * player.direction, -22);
            Projectile.velocity = RedeHelper.PolarVector(1, player.direction == -1 ? (float)Math.PI - 0.3f : 0.3f);

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            ++AITimer;

            if (AITimer >= 80)
            {
                CastLights(new Vector3(1f, 0.7f, 0f));
                if (Main.rand.NextBool() && (AITimer - 80) % 2 == 0)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), endPoint, new Vector2(0), ModContent.ProjectileType<MagnifyingGlassRayRemain>(), Projectile.damage, 0, Projectile.owner, Projectile.whoAmI);
                if (Main.dayTime)
                    ParticleManager.NewParticle(endPoint, RedeHelper.PolarVector(1, -Projectile.rotation), new EmberParticle(), Color.White, 1);
                else
                    ParticleSystem.NewParticle<BlueEmberParticle>(endPoint, RedeHelper.PolarVector(1, -Projectile.rotation), Color.White, 1);
            }

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
            #region length
            // code from slr
            for (int k = 0; k < MaxLaserLength; k++)
            {
                Vector2 posCheck = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * k * 8;

                if (Helper.PointInTile(posCheck) || k == MaxLaserLength - 1)
                {
                    endPoint = posCheck;
                    break;
                }
            }

            LaserLength = LengthSetting(Projectile, endPoint);
            #endregion

            if (Main.myPlayer != player.whoAmI)
                CheckHits();
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) // code from slr
        {
            if (LaserLength >= 1f && Helper.CheckLinearCollision(Projectile.Center, endPoint, targetHitbox, out Vector2 colissionPoint))
                return true;

            return false;
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
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle((int)(64 * Frame), LaserEndSegmentLength, 64, LaserSegmentLength), color, r,
                    new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle((int)(64 * Frame), LaserEndSegmentLength, 64, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition,
                new Rectangle((int)(64 * Frame), 0, 64, LaserEndSegmentLength), color, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 5) * (1 / scale) * unit - Main.screenPosition,
                new Rectangle((int)(64 * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition,
                new Rectangle((int)(64 * Frame), 0, 64, LaserEndSegmentLength), color * 0.5f, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + (maxDist + 5) * (1 / scale) * unit - Main.screenPosition,
                new Rectangle((int)(64 * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(64 / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 0.8f, 1f, 0.8f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength,
                Projectile.GetAlpha(Main.dayTime ? Color.White : Color.CornflowerBlue) * opacity, (int)FirstSegmentDrawDist);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength + 5) - Main.screenPosition;
            Color colour = Main.dayTime ? new(249, 240, 161) : Color.CornflowerBlue;

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin, Main.rand.NextFloat(.9f, 1.3f), 0, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * Main.rand.NextFloat(1f, 0.7f), 0, origin, Main.rand.NextFloat(.9f, 1.3f), 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
            return false;
        }
        #endregion
    }
}
