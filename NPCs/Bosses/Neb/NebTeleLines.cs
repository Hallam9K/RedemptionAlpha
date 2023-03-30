using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class NebTeleLine1 : ModProjectile
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
            Projectile.timeLeft = 200;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            if (!npc.active || (npc.type != ModContent.NPCType<Nebuleus>() && npc.type != ModContent.NPCType<Nebuleus_Clone>() && npc.type != ModContent.NPCType<Nebuleus2>() && npc.type != ModContent.NPCType<Nebuleus2_Clone>()))
                Projectile.Kill();

            Projectile.Center = npc.Center;
            if (AITimer < Projectile.ai[0] - 40)
                Projectile.velocity = npc.DirectionTo(Main.player[npc.target].Center + Main.player[npc.target].velocity * 20f);

            if (AITimer >= Projectile.ai[0])
            {
                float timer = AITimer - 60f;
                Projectile.alpha = (int)MathHelper.Lerp(100f, 255f, timer / 30f);
            }

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 1;

            if (Projectile.timeLeft >= 50)
            {
                Projectile.alpha -= 5;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 100, 255);
            }
            if (Projectile.timeLeft <= 20)
            {
                if (Projectile.timeLeft > 20)
                    Projectile.timeLeft = 20;

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
    public class NebTeleLine2 : ModProjectile
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
            Projectile.timeLeft = 80;
            Projectile.alpha = 255;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 1;

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
}