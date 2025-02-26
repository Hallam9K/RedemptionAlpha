using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;


namespace Redemption.Projectiles.Magic
{
    public class SunshardRay : LaserProjectile
    {
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Sunshard Ray");
            ElementID.ProjHoly[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 60;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;

            LaserSegmentLength = 14;
            LaserWidth = 20;
            LaserEndSegmentLength = 14;
            NewCollision = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 positionInWorld = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
            ParticleOrchestraSettings particleOrchestraSettings = default;
            particleOrchestraSettings.PositionInWorld = positionInWorld;
            ParticleOrchestraSettings settings = particleOrchestraSettings;
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.PaladinsHammer, settings, Projectile.owner);
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.PaladinsHammer, settings, Projectile.owner);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = RedeHelper.PolarVector(1, Projectile.rotation);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 0.1f;
            else
                Projectile.Center = playerCenter + Projectile.velocity * 50f;

            if (AITimer <= 10)
            {
                LaserScale += 0.09f;
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

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength - LaserSegmentLength * 2, Color.White, (int)FirstSegmentDrawDist);

            Texture2D flare = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength - Main.screenPosition;
            Color colour = new(249, 240, 161);

            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour), AITimer / 10, origin, 1.5f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), Projectile.GetAlpha(colour), -(AITimer / 10), origin, 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
    }
}
