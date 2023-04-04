using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Audio;
using Redemption.Projectiles.Ranged;
using Redemption.BaseExtension;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Globals.Player;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class XeniumElectrolaser_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Ranged/XeniumElectrolaser";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Electrolaser");
        }
        public override void SetSafeDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }
        private float offset;
        private float rotOffset;
        private float shake;
        private float alphaTimer;
        private float glowTimer;
        private float shotOff2 = 0.4f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;

                Vector2 vector13 = Main.MouseWorld - vector;

                vector13.Normalize();
                if (vector13.HasNaNs())
                    vector13 = Vector2.UnitX * player.direction;

                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = vector13;
                if (player.noItems || player.CCed || player.dead || !player.active)
                    Projectile.Kill();

                Projectile.netUpdate = true;
            }
            Projectile.Center = player.MountedCenter;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(31 * Projectile.spriteDirection, Projectile.rotation);
            offset -= 5;
            rotOffset += 0.1f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[1] == 1)
                {
                    if (Projectile.localAI[0]++ <= 0)
                    {
                        SoundEngine.PlaySound(CustomSounds.Zap2 with { Pitch = 0.3f }, player.position);
                        player.RedemptionScreen().ScreenShakeIntensity += 3;
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 5;
                        player.velocity -= RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());
                        offset = 25;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation()), ModContent.ProjectileType<XeniumElectrolaser_Beam2>(), Projectile.damage, Projectile.knockBack, player.whoAmI);

                        player.RedemptionScreen().ScreenShakeIntensity += 2 + Projectile.localAI[0] / 30;
                        if (Projectile.localAI[0] >= 60)
                            player.velocity -= RedeHelper.PolarVector(1 + Projectile.localAI[0] / 120, (Main.MouseWorld - player.Center).ToRotation());
                    }
                    else
                    {
                        if (Projectile.localAI[0] % 5 == 0)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation()), ModContent.ProjectileType<XeniumElectrolaser_Beam2>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 1);

                        if (Projectile.localAI[0]++ >= 30)
                            Projectile.Kill();
                    }
                }
                else
                {
                    if (player.channel && player.GetModPlayer<EnergyPlayer>().statEnergy >= 2)
                    {
                        glowTimer++;
                        for (int k = 0; k < 1 + (Projectile.localAI[0] / 120); k++)
                        {
                            Vector2 dustPos = Projectile.Center + RedeHelper.PolarVector(52 * Projectile.spriteDirection, Projectile.rotation);
                            Vector2 dVector;
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            dVector.X = (float)(Math.Sin(angle) * 20);
                            dVector.Y = (float)(Math.Cos(angle) * 20);
                            ParticleManager.NewParticle(dustPos + dVector, (dustPos + dVector).DirectionTo(dustPos) * 1f, new LightningParticle(), Color.White, 1f, 5);
                        }
                        shake += 0.02f;
                        Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));

                        if (Projectile.localAI[0]++ > 0 && Projectile.localAI[0] % 20 == 0 && Projectile.localAI[0] < 180)
                        {
                            player.GetModPlayer<EnergyPlayer>().statEnergy -= 2;
                            SoundEngine.PlaySound(CustomSounds.Zap2 with { Pitch = 0.3f }, player.position);
                            SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch with { Volume = 0.6f }, Projectile.position);

                            offset = 10;
                            rotOffset = -0.02f;
                            float shotOff = MathHelper.Max(shotOff2, 0);
                            for (int i = 0; i < Main.rand.Next(4, 6); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation() + Main.rand.NextFloat(-shotOff, shotOff)), ModContent.ProjectileType<XeniumElectrolaser_Beam>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, player.whoAmI, 1);
                            }
                            shotOff2 -= 0.06f;
                        }
                        if (Projectile.localAI[0] >= 180)
                            player.channel = false;
                        if (Projectile.localAI[0] % 40 == 0)
                        {
                            SoundEngine.PlaySound(CustomSounds.ShieldActivate with { Volume = 0.6f, Pitch = Projectile.localAI[0] / 300 }, Projectile.position);
                            player.GetModPlayer<EnergyPlayer>().statEnergy -= 3;
                        }
                    }
                    else if (Projectile.localAI[0] >= 180 && player.GetModPlayer<EnergyPlayer>().statEnergy >= 2)
                    {
                        if (Projectile.localAI[0] < 400)
                        {
                            if (player.GetModPlayer<EnergyPlayer>().statEnergy > 0)
                            {
                                if (Projectile.localAI[0] > 120)
                                {
                                    SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                                    SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
                                }
                                else
                                    SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch with { Volume = 0.6f }, Projectile.position);

                                offset = 20;
                                rotOffset = -0.08f;
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation()), ModContent.ProjectileType<XeniumElectrolaser_Beam>(), Projectile.damage * 5, Projectile.knockBack, player.whoAmI, 40);

                                player.RedemptionScreen().ScreenShakeIntensity += 20;
                                player.velocity -= RedeHelper.PolarVector(3, (Main.MouseWorld - player.Center).ToRotation());
                            }
                            Projectile.localAI[0] = 400;
                        }
                        else
                        {
                            if (Projectile.localAI[0]++ >= 420)
                                Projectile.Kill();
                        }
                    }
                    else
                        Projectile.Kill();
                }
            }
            shake = MathHelper.Min(shake, 3f);
            offset = MathHelper.Clamp(offset, 0, 40);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
            if (alphaTimer++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-26 + offset, Projectile.velocity.ToRotation());
            float timerMax = 180;
            int timerProgress = (int)(glow.Width * (glowTimer / timerMax));

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(new Rectangle(0, 0, timerProgress, glow.Height)), Projectile.GetAlpha(RedeColor.GreenPulse), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin + new Vector2(Projectile.spriteDirection == -1 ? -30 : -50, -12), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}