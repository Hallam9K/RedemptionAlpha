using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.Audio;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_NormalBeam : LaserProjectile
    {
        private new const float FirstSegmentDrawDist = 12;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Beam");
        }

        public override void SetSafeDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            LaserScale = 1;
            LaserSegmentLength = 22;
            LaserWidth = 22;
            LaserEndSegmentLength = 22;
            MaxLaserLength = 1760;
        }
        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            if (AITimer == 0)
            {
                if (Projectile.ai[1] > 2)
                    Projectile.timeLeft = 180;

                if (Projectile.ai[1] == 4)
                    offsetLeft = true;

                LaserScale = 0.1f;
            }

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active)
                Projectile.Kill();

            Vector2 EyePos = npc.Center + RedeHelper.PolarVector(36, npc.rotation - (float)Math.PI / 2) + RedeHelper.PolarVector(npc.spriteDirection == -1 ? -30 : -14, npc.rotation);
            Projectile.Center = EyePos;

            if (Projectile.ai[1] > 2)
            {
                if (originalVelocity == Vector2.Zero)
                    originalVelocity = Projectile.velocity;

                if (offsetLeft)
                {
                    vectorOffset -= 0.02f;
                    if (vectorOffset <= -0.6f)
                    {
                        vectorOffset = -0.6f;
                        offsetLeft = false;
                    }
                }
                else
                {
                    vectorOffset += 0.02f;
                    if (vectorOffset >= 0.6f)
                    {
                        vectorOffset = 0.6f;
                        offsetLeft = true;
                    }
                }
                float velRot = BaseUtility.RotationTo(Projectile.Center, Projectile.Center + originalVelocity);
                Projectile.velocity = BaseUtility.RotateVector(default, new Vector2(Projectile.velocity.Length(), 0f), velRot + (vectorOffset * 0.5f));
            }
            int start = Projectile.ai[1] > 2 || Projectile.ai[1] == -1 ? 50 : 30;
            if (AITimer == start)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.BallFire, Projectile.position);
                if (Projectile.ai[1] == 1)
                    Projectile.velocity = new Vector2(1 * npc.spriteDirection, 0.01f);
                else if (Projectile.ai[1] == 2)
                    Projectile.velocity = new Vector2(1 * npc.spriteDirection, -0.01f);
            }
            if (AITimer >= start && AITimer <= start + 10)
            {
                Projectile.hostile = true;
                LaserScale += 0.09f;
            }
            else if (Projectile.timeLeft < 10 || !npc.active)
            {
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;

                LaserScale -= 0.1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            #endregion

            LaserLength = MaxLaserLength;
            ++AITimer;
        }
        #region Drawcode
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.8f, 1.2f, 1.8f, 1.2f);
            float r = unit.ToRotation() + rotation;
            // Draws the Laser 'body'
            for (float i = transDist; i <= (maxDist * (1 / LaserScale)); i += LaserSegmentLength)
            {
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);

                Main.EntitySpriteDraw(texture, origin - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), color * 0.5f, r,
                    new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            }
            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale, 0, 0);

            // Draws the Laser 'base'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
            // Draws the Laser 'end'
            Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), color * 0.5f, r, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), scale * new Vector2(pulse, 1), 0, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(RedeColor.RedPulse), (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
    }
}