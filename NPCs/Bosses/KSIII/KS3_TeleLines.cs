using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Globals;
using System;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_TeleLine1 : ModProjectile
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
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 160;
            Projectile.alpha = 255;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (npc.active && npc.type == ModContent.NPCType<KS3>())
            {
                Projectile.Center = npc.Center + RedeHelper.PolarVector(54, (npc.ModNPC as KS3).gunRot) + RedeHelper.PolarVector(13 * npc.spriteDirection,
                    (npc.ModNPC as KS3).gunRot - (float)Math.PI / 2);
                Projectile.velocity = RedeHelper.PolarVector(10, (npc.ModNPC as KS3).gunRot);
            }
            else if (npc.active && npc.type == ModContent.NPCType<KS3_Clone>())
            {
                Projectile.Center = npc.Center + RedeHelper.PolarVector(54, (npc.ModNPC as KS3_Clone).gunRot) + RedeHelper.PolarVector(13 * npc.spriteDirection, (npc.ModNPC as KS3_Clone).gunRot - (float)Math.PI / 2);
                Projectile.velocity = RedeHelper.PolarVector(10, (npc.ModNPC as KS3_Clone).gunRot);
            }

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 1;

            if (Projectile.timeLeft >= 40)
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
            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.Cyan), (int)FirstSegmentDrawDist);
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
    public class KS3_TeleLine2 : ModProjectile
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
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 70;
            Projectile.alpha = 255;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.Center = npc.Center;
            Projectile.velocity = npc.DirectionTo(Main.player[npc.target].Center + Main.player[npc.target].velocity * 20f);

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 1;

            if (Projectile.timeLeft >= 40)
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
            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.Cyan), (int)FirstSegmentDrawDist);
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