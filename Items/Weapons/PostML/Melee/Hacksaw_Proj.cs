using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;
using Redemption.Projectiles.Melee;
using Redemption.Base;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Hacksaw_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Automated Hacksaw");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 3;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[1] >= 1 ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;
        private int spinFrame;

        public override void AI()
        {
            if (Projectile.frameCounter++ % 5 == 0)
            {
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
            if (Projectile.frameCounter % 3 == 0)
            {
                if (++spinFrame > 3)
                    spinFrame = 0;
            }
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Projectile.ai[1] == 0)
                        {
                            swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                            Projectile.ai[1] = 1;
                            oldRotation = swordRotation;
                            directionLock = player.direction;
                        }
                        else if (Projectile.ai[1] >= 1)
                        {
                            player.direction = directionLock;
                            Projectile.ai[1]++;
                            float timer = Projectile.ai[1] - 1;
                            if (timer % 20 == 0)
                                SoundEngine.PlaySound(SoundID.Item22, Projectile.position);

                            swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / 5f / SwingSpeed);

                            if (Projectile.ai[1] >= 14 * SwingSpeed && !player.channel)
                                Projectile.Kill();
                        }
                        break;
                }
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 40f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects spriteEffects2 = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Texture2D spinTex = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Effect").Value;
            int height = texture.Height / 2;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            int spinHeight = spinTex.Height / 4;
            int spinY = spinHeight * spinFrame;
            Rectangle spinRect = new(0, spinY, spinTex.Width, spinHeight);
            Vector2 spinOrigin = new(spinTex.Width / 2, spinHeight / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.LightBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color * 0.5f, oldrot[k], drawOrigin, Projectile.scale, spriteEffects, 0);
            }
            Main.EntitySpriteDraw(spinTex, player.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(spinRect), Projectile.GetAlpha(Color.White) * 0.5f, Projectile.rotation, spinOrigin, Projectile.scale + 0.2f, spriteEffects2, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}