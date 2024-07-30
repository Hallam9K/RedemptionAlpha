using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OmegaMegaBeam : LaserProjectile
    {
        private new float FirstSegmentDrawDist = 40;
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Obliterator Beam");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 156;
            Projectile.height = 156;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            LaserScale = 1;
            LaserSegmentLength = 96;
            LaserWidth = 156;
            LaserEndSegmentLength = 96;
            MaxLaserLength = 1920;
            maxLaserFrames = 3;
            StopsOnTiles = false;
        }
        private float offset;
        private Vector2 origin;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region Beginning And End Effects
            if (AITimer == 0)
            {
                origin = Projectile.Center;
                LaserScale = 0.1f;
            }

            NPC npc = null;
            if (Projectile.ai[0] != -1)
            {
                npc = Main.npc[(int)Projectile.ai[0]];

                Vector2 LaserPos = new(npc.position.X + (npc.spriteDirection == -1 ? (46 - offset) : (16 + offset)), npc.position.Y + 70);
                Projectile.Center = LaserPos;
            }
            else
            {
                Vector2 LaserPos = new(origin.X, origin.Y - offset);
                Projectile.Center = LaserPos;
            }


            if (AITimer == 48)
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.position);
            if (AITimer >= 48 && AITimer < 160 && Projectile.ai[0] != -1 && npc != null)
                Main.player[npc.target].GetModPlayer<ScreenPlayer>().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 40);
            if (AITimer > 48 && AITimer <= 60)
            {
                offset -= Main.getGoodWorld ? 12 : 4;
                Projectile.hostile = true;
                if (Main.getGoodWorld)
                    LaserScale += 0.27f;
                else
                    LaserScale += 0.09f;
            }
            else if (Projectile.timeLeft < 10 || (Projectile.ai[0] != -1 && npc != null && !npc.active))
            {
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;
                Projectile.hostile = false;
                offset += Main.getGoodWorld ? 15 : 5;
                if (Main.getGoodWorld)
                    LaserScale -= 0.3f;
                else
                    LaserScale -= 0.1f;
            }

            LaserScale = MathHelper.Clamp(LaserScale, 0.1f, Main.getGoodWorld ? 3 : 1);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            #endregion

            LaserLength = MaxLaserLength;
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
        }
        #region Drawcode
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float pulse = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.4f, 1.2f, 1.4f, 1.2f);
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
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.SolarDye);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            DrawLaser(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * LaserScale), new Vector2(1f, 0).RotatedBy(Projectile.rotation) * LaserScale, -1.57f, LaserScale, LaserLength, Projectile.GetAlpha(Color.White), (int)FirstSegmentDrawDist);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        #endregion
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback.Base += 14;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            info.Dodgeable = false;
        }
    }
}