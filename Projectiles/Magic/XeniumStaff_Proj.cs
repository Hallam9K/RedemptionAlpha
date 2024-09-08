using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class XeniumStaff_Proj : LaserProjectile
    {
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Ray");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetSafeDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            NewCollision = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.itemAnimation = 2;
            player.itemTime = 2;
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = RedeHelper.PolarVector(1f, Projectile.rotation);

            for (int i = 0; i < 2; i++)
            {
                int num5 = Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength - new Vector2(4, 4), 8, 8, ModContent.DustType<GlowDust>(), Scale: .7f);
                Color dustColor = new(151, 255, 182) { A = 0 };
                if (Main.rand.NextBool())
                    dustColor = new(3, 249, 51) { A = 0 };
                Main.dust[num5].velocity = -Projectile.velocity * Main.rand.NextFloat(.1f, .3f);
                Main.dust[num5].color = dustColor * Projectile.Opacity;
                Main.dust[num5].noGravity = true;
            }

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 0.1f;
            else
                Projectile.Center = playerCenter + Vector2.Normalize(Projectile.velocity) * 48f;

            if (AITimer <= 10)
            {
                LaserScale += 0.09f;
            }
            else if (Projectile.timeLeft < 10 || !player.active)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }
                LaserScale -= 0.1f;
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

            ++AITimer;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active || p.type != ModContent.ProjectileType<XeniumBubble_Proj>())
                    continue;

                Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
                float point = 0f;
                if (Collision.CheckAABBvLineCollision(p.Hitbox.TopLeft(), p.Hitbox.Size(), Projectile.Center,
                    Projectile.Center + unit * LaserLength, 20 * LaserScale, ref point))
                {
                    p.ai[0] = 1;
                    p.Kill();
                }
            }
            if (Main.myPlayer != player.whoAmI)
                CheckHits();
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
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition,
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition,
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength - LaserSegmentLength, Color.White, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
    }
}
