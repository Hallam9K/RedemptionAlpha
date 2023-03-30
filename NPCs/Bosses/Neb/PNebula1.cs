using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Redemption.Base;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Neb
{
    public class PNebula1_Tele : ModProjectile
    {
        public override string Texture => "Redemption/Textures/TelegraphLine";
        public float AITimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float LaserLength = 0;
        public float LaserScale = 1;
        public int LaserSegmentLength = 10;
        public int LaserWidth = 1;
        public int LaserEndSegmentLength = 10;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 5;

        public int MaxLaserLength = 2000;
        public bool StopsOnTiles = false;
        // >
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph");
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 80;
            Projectile.alpha = 255;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            if (AITimer == 0)
            {
                LaserScale = 1;
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula1>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula2>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PNebula3>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }
            if (Projectile.timeLeft >= 50)
            {
                Projectile.alpha -= 5;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 100, 255);
            }
            if (Projectile.timeLeft <= 30)
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;

                Projectile.alpha += 10;
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
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle(LaserWidth, LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle(LaserWidth, 0, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle(LaserWidth, LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.LightPink), (int)FirstSegmentDrawDist);
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

    public class PNebula1 : ModProjectile
    {
        public int proType = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Piercing Nebula");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 30)
                SoundEngine.PlaySound(SoundID.Item125, Projectile.position);
            if (Projectile.localAI[0] >= 30)
            {
                if (proType != 0)
                {
                    ParticleManager.NewParticle(Projectile.Center, Vector2.Zero, new GlowParticle2(), Color.Pink, 0.6f, .45f, Main.rand.Next(50, 60));
                }

                if (originalVelocity == Vector2.Zero)
                {
                    originalVelocity = Projectile.velocity;
                }
                if (proType != 0)
                {
                    if (offsetLeft)
                    {
                        vectorOffset -= 0.5f;
                        if (vectorOffset <= -1.3f)
                        {
                            vectorOffset = -1.3f;
                            offsetLeft = false;
                        }
                    }
                    else
                    {
                        vectorOffset += 0.5f;
                        if (vectorOffset >= 1.3f)
                        {
                            vectorOffset = 1.3f;
                            offsetLeft = true;
                        }
                    }
                    float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                    Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
                }
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
                Projectile.spriteDirection = 1;
                Projectile.hostile = true;
            }
            else
                Projectile.hostile = false;
        }
        public override bool ShouldUpdatePosition()
        {
            if (Projectile.localAI[0] >= 30)
                return true;

            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] >= 30)
            {
                if (proType == 0)
                {
                    Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
                    for (int k = 0; k < Projectile.oldPos.Length; k++)
                    {
                        Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                        Color color = Projectile.GetAlpha(new Color(255, 255, 255, 0)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                        Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                    }
                    Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                    return false;
                }
                return true;
            }
            else
                return false;
        }
    }
    public class PNebula2 : PNebula1
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 1;
            offsetLeft = false;
        }
    }
    public class PNebula3 : PNebula1
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Piercing Nebula");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            proType = 2;
            offsetLeft = true;
        }
    }
}