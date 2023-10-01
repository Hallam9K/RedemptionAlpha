using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Redemption.NPCs.Bosses.ADD;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Dusts;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    public class GolemEyeRay : ModProjectile
    {
        public float AITimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float Frame
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }
        public float LaserLength = 0;
        public float LaserScale = 0;
        public int LaserSegmentLength = 14;
        public int LaserWidth = 12;
        public int LaserEndSegmentLength = 14;

        //should be set to about half of the end length
        private const float FirstSegmentDrawDist = 7;

        public int MaxLaserLength = 2000;
        public int maxLaserFrames = 1;
        public int LaserFrameDelay = 5;
        public bool StopsOnTiles = true;
        // >
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye Ray");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = LaserWidth;
            Projectile.height = LaserWidth;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.Redemption().ParryBlacklist = true;
        }

        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 origin = host.Center - new Vector2(-2 * host.spriteDirection, 18);
            if (host.ai[0] == 3)
                origin = host.Center;

            if (Projectile.timeLeft > 10)
            {
                for (int i = 0; i < 2; i++)
                {
                    int num5 = Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength + 10) - new Vector2(3, 3), 6, 6, ModContent.DustType<GlowDust>(), 0, 0, Scale: .6f);
                    Color dustColor = new(253, 216, 178) { A = 0 };
                    if (Main.rand.NextBool())
                        dustColor = new(243, 155, 86) { A = 0 };
                    Main.dust[num5].velocity *= .01f;
                    Main.dust[num5].color = dustColor * Projectile.Opacity;
                    Main.dust[num5].noGravity = true;
                }
            }
            #region Beginning And End Effects
            if (AITimer == 0)
            {
                if (host.type == ModContent.NPCType<EaglecrestGolem2>() && host.ai[0] == 3)
                    Projectile.timeLeft = 20;
                LaserScale = 0.1f;
            }
            else
                Projectile.Center = origin - Vector2.Normalize(Projectile.velocity) * 16f;

            if (host.type == ModContent.NPCType<EaglecrestGolem2>() && host.ai[0] != 3)
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.024f * host.spriteDirection);
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.01f * host.spriteDirection);

            if (AITimer <= 10)
            {
                LaserScale += 0.09f;
            }
            else if (Projectile.timeLeft < 10 || !host.active)
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

            #region Frame and Timer Updates
            /*++Projectile.frameCounter;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                {
                    Frame = 0;
                }
            }*/
            ++AITimer;
            #endregion

            #region misc
            //CutTiles();
            //CastLights();
            #endregion
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
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }
        #endregion

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
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Color.White, (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
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