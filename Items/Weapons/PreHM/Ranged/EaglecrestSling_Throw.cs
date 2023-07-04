using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Ranged;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class EaglecrestSling_Throw : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eaglecrest Sling");
            Main.projFrames[Projectile.type] = 6;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override bool? CanHitNPC(NPC target) => false ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    if (Projectile.localAI[0]++ == 0)
                    {
                        directionLock = player.direction;
                        oldRotation = MathHelper.ToRadians(-45f * player.direction - 90f);
                    }

                    player.direction = directionLock;

                    if (Projectile.localAI[0] % 20 == 0)
                        SoundEngine.PlaySound(SoundID.Item19, Projectile.position);

                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + Projectile.velocity * 40f,
                            RedeHelper.PolarVector(20, Projectile.rotation + (player.direction == 1 ? MathHelper.PiOver4 : MathHelper.Pi)),
                            ModContent.ProjectileType<EaglecrestSling_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                else if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;

                    Projectile.localAI[0]++;

                    if (++Projectile.frameCounter >= 5)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 5)
                            Projectile.Kill();
                    }
                }
            }

            float slingRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), Projectile.localAI[0] / 10f);

            Projectile.velocity = slingRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = playerCenter + Projectile.velocity * 40f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}