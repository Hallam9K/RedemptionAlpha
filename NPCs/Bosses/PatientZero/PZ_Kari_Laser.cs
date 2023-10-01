using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Redemption.Base;
using Terraria.ID;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Redemption.Buffs.Debuffs;
using Terraria.ModLoader;
using Redemption.Dusts;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Kari_Laser : LaserProjectile
    {
        private new readonly float FirstSegmentDrawDist = 23;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Laser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
            LaserScale = 0.1f;
            LaserSegmentLength = 46;
            LaserWidth = 32;
            LaserEndSegmentLength = 46;
            MaxLaserLength = 1840;
            maxLaserFrames = 3;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 80);
        public override bool CanHitPlayer(Player target) => AITimer >= 20;
        public override bool? CanHitNPC(NPC target) => target.friendly && AITimer >= 20 ? null : false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch);
            Main.dust[dust].velocity = RedeHelper.PolarVector(6, Projectile.rotation);
            Main.dust[dust].noGravity = true;

            #region Beginning And End Effects
            if (AITimer == 0)
                LaserScale = 0.1f;

            if (AITimer >= 20)
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, 255);
                if (Projectile.timeLeft > 10)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int num5 = Dust.NewDust(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (LaserLength + 30) - new Vector2(5, 5), 10, 10, ModContent.DustType<GlowDust>(), 0, 0, Scale: 1.4f);
                        Color dustColor = new(219, 247, 171) { A = 0 };
                        if (Main.rand.NextBool())
                            dustColor = new(88, 204, 92) { A = 0 };
                        Main.dust[num5].velocity = -Projectile.velocity * Main.rand.NextFloat(.1f, .3f);
                        Main.dust[num5].color = dustColor * Projectile.Opacity;
                        Main.dust[num5].noGravity = true;
                    }
                }
            }
            else
            {
                Projectile.alpha -= 10;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 180, 255);
            }
            if (AITimer <= 10)
                LaserScale += 0.09f;
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            Projectile.Center = npc.Center;
            if (Projectile.timeLeft < 10 || !npc.active)
            {
                Projectile.hostile = false;
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;

                LaserScale -= 0.1f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

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

            ++Projectile.frameCounter;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                    Frame = 0;
            }
            ++AITimer;
        }

        #region Laser AI Submethods
        public override void EndpointTileCollision()
        {
            for (LaserLength = FirstSegmentDrawDist; LaserLength < MaxLaserLength; LaserLength += LaserSegmentLength)
            {
                Vector2 start = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength;
                Rectangle box = new((int)(RedeGen.LabVector.X + 109) * 16, (int)(RedeGen.LabVector.Y + 170) * 16, 69 * 16, 41 * 16);
                if (!box.Intersects(new Rectangle((int)start.X, (int)start.Y, 1, 1)))
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

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(AITimer >= 20 ? Color.White : Color.Red), (int)FirstSegmentDrawDist);

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
                Projectile.Center + unit * (LaserLength - 20), 30 * LaserScale, ref point))
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