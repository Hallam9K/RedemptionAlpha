using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_BeamCell : LaserProjectile
    {
        private new const float FirstSegmentDrawDist = 5;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Beam Cell");
            ElementID.ProjThunder[Type] = true;
        }

        public override void SetSafeDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            LaserLength = 0;
            LaserScale = 0;
            LaserSegmentLength = 34;
            LaserWidth = 22;
            LaserEndSegmentLength = 10;
            MaxLaserLength = 2000;
            maxLaserFrames = 2;
            LaserFrameDelay = 10;
            StopsOnTiles = false;
        }

        private bool faceLeft;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != ModContent.NPCType<KS3>() && npc.type != ModContent.NPCType<KS3_Clone>()))
                Projectile.Kill();

            if (AITimer == 0)
            {
                LaserScale = 0.1f;
                if (npc.spriteDirection != 1)
                    faceLeft = true;
            }

            Vector2 CellPos = new(npc.Center.X + 2 * npc.spriteDirection, npc.Center.Y - 16);
            Projectile.Center = CellPos;
            Projectile.velocity = Projectile.velocity.RotatedBy(0.01f * npc.spriteDirection);
            if (npc.spriteDirection == 1 && faceLeft)
            {
                Projectile.velocity *= -1;
                faceLeft = false;
            }
            else if (npc.spriteDirection != 1 && !faceLeft)
            {
                Projectile.velocity *= -1;
                faceLeft = true;
            }

            if (AITimer <= 10)
            {
                LaserScale += 0.09f;
            }
            else if (Projectile.timeLeft < 10 || !npc.active)
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
            ++Projectile.frameCounter;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                {
                    Frame = 0;
                }
            }
            ++AITimer;
            #endregion
        }
        #region Drawcode
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
    }
}