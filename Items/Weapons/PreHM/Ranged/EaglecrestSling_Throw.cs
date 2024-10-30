using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Projectiles.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            Projectile.extraUpdates = 4;
        }

        public override bool? CanHitNPC(NPC target) => false ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        public ref float Timer => ref Projectile.localAI[0];
        public Vector2 startVector;
        public Vector2 vector;
        public Vector2 launchDirection;
        public float Rot;
        public float initialLength;
        public float initialRot;
        public float acc;
        public bool launch;
        public bool parallel;
        public bool rhythm;
        public bool success;
        public float strLength;
        public int swingDir;
        public int bonus;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            if (player.HeldItem.ModItem is EaglecrestSling sling)
            {
                acc = sling.shot * 0.1f + 1;
                bonus = sling.shot + 1;
            }

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.spriteDirection = player.direction;
            Projectile.rotation = (playerCenter - Projectile.Center).ToRotation() - MathHelper.PiOver2;
            Projectile.velocity = RedeHelper.PolarVector(1 * player.direction, Projectile.rotation);
            Projectile.Center = playerCenter + vector;
            Rot = MathHelper.ToRadians(Timer * 2 * acc) * directionLock;
            vector = startVector.RotatedBy(Rot) * 40;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (playerCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);

            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    if (Timer++ == 0)
                    {
                        directionLock = player.direction;
                        oldRotation = MathHelper.ToRadians(-45f * player.direction - 90f);
                        initialRot = (Projectile.Center - playerCenter).ToRotation();
                        startVector = RedeHelper.PolarVector(1, initialRot);
                    }

                    player.direction = directionLock;

                    if (LaunchAngleCheck())
                        parallel = true;
                    else
                        parallel = false;

                    if (!success)
                    {
                        if (Timer * 2 * acc % 360 >= 60 && Timer * 2 * acc % 360 <= 150)
                        {
                            if (!rhythm)
                            {
                                SoundEngine.PlaySound(SoundID.Item19 with { Volume = 2f, Pitch = -.4f }, player.position);
                                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 1.2f, Volume = 0.4f }, player.position);
                                RedeDraw.SpawnExplosion(player.MountedCenter + vector, Color.Yellow, scale: 1, noDust: true, shakeAmount: 0, tex: "Redemption/Textures/WhiteFlare");
                                RedeDraw.SpawnExplosion(player.MountedCenter + vector, Color.White, scale: .8f, noDust: true, shakeAmount: 0, tex: "Redemption/Textures/WhiteFlare");
                                DustHelper.DrawCircle(player.Center, 64, 4, 1, 1, 1, 2, nogravity: true);
                                rhythm = true;
                            }
                        }
                        else
                            rhythm = false;
                    }

                    if (!player.channel)
                    {
                        if (rhythm || success)
                        {
                            success = true;
                            if (parallel)
                            {
                                Projectile.ai[0] = 1;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(Main.MouseWorld) * 40, ModContent.ProjectileType<EaglecrestSling_Proj>(), Projectile.damage * bonus, Projectile.knockBack, Projectile.owner);
                            }
                        }
                        else
                        {
                            Projectile.ai[0] = 1;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 40, ModContent.ProjectileType<EaglecrestSling_Proj>(), Projectile.damage * bonus, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                else if (Projectile.ai[0] >= 1)
                {
                    player.direction = directionLock;

                    Timer++;

                    if (++Projectile.frameCounter >= 25)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;
                        if (Projectile.frame > 5)
                            Projectile.Kill();
                    }
                }
            }
        }
        private bool LaunchAngleCheck()
        {
            Vector2 cursor = Main.MouseWorld - Projectile.Center;
            Vector2 launchDir = Projectile.velocity;
            float num = cursor.ToRotation() - launchDir.ToRotation();
            if (Math.Abs(num) <= MathHelper.ToRadians(30))
                return true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition,
                new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition,
                new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}