using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Particles;
using Redemption.Projectiles.Ranged;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

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
        bool fakeKill;
        Vector2 stayPos;
        Vector2 stayVel;
        private ref float Timer => ref Projectile.localAI[0];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (!fakeKill)
                RedeProjectile.HoldOutProjBasics(Projectile, player, vector);
            Projectile.Center = vector;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            if (!fakeKill)
            {
                player.ChangeDir(Projectile.direction);
                player.heldProj = Projectile.whoAmI;
                player.itemTime = 2;
                player.itemAnimation = 2;
                player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            }

            float num = 0;
            if (Projectile.spriteDirection == -1)
                num = MathHelper.Pi;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;

            Vector2 gunPos = Projectile.Center + RedeHelper.PolarVector(31 * Projectile.spriteDirection, Projectile.rotation);
            offset -= 5;
            rotOffset += 0.1f;
            if (Projectile.ai[1] == 1)
            {
                if (Timer++ <= 0)
                {
                    stayPos = gunPos;
                    stayVel = RedeHelper.PolarVector(3, Projectile.velocity.ToRotation());
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Zap2 with { Pitch = 0.3f }, player.position);
                    player.RedemptionScreen().ScreenShakeIntensity += 3;
                    player.GetModPlayer<EnergyPlayer>().statEnergy -= 5;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        player.velocity -= RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), stayPos, stayVel, ProjectileType<XeniumElectrolaser_Beam2>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    }
                    offset = 25;

                    player.RedemptionScreen().ScreenShakeIntensity += 2 + Timer / 30;
                    if (Timer >= 60)
                        player.velocity -= RedeHelper.PolarVector(1 + Timer / 120, (Main.MouseWorld - player.Center).ToRotation());

                    float angle = (Projectile.Center - Main.MouseWorld).ToRotation();
                    Vector2 position = Projectile.Center - RedeHelper.PolarVector(30, angle);
                    Color bright = Color.Multiply(new(186, 255, 185, 0), 1);
                    for (int j = 0; j < 8; j++)
                    {
                        float randomRotation = Main.rand.NextFloat(-0.5f, 0.5f);
                        float randomVel = Main.rand.NextFloat(1.5f, 3);
                        RedeParticleManager.CreateSpeedParticle(position, Projectile.velocity.RotatedBy(randomRotation) * randomVel * 16, 1, bright);
                    }
                }
                else
                {
                    if (Timer % 5 == 0 && Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), stayPos, stayVel, ProjectileType<XeniumElectrolaser_Beam2>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 1);
                    }

                    if (Timer++ == 30)
                        fakeKill = true;
                    if (Timer >= 180)
                        Projectile.Kill();
                }
            }
            else
            {
                if (player.channel && player.GetModPlayer<EnergyPlayer>().statEnergy >= 2)
                {
                    glowTimer++;
                    for (int k = 0; k < 1 + (Timer * player.GetAttackSpeed(DamageClass.Ranged) / 120); k++)
                    {
                        Vector2 dustPos = Projectile.Center + RedeHelper.PolarVector(52 * Projectile.spriteDirection, Projectile.rotation);
                        Vector2 dVector;
                        double angle = Main.rand.NextDouble() * 2d * Math.PI;
                        dVector.X = (float)(Math.Sin(angle) * 20);
                        dVector.Y = (float)(Math.Cos(angle) * 20);
                        RedeParticleManager.CreateLightningParticle(dustPos + dVector, (dustPos + dVector).DirectionTo(dustPos) * 1f, 1f, RedeParticleManager.greenColors);
                    }
                    shake += 0.02f;
                    Projectile.position += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));

                    if (Timer++ > 0 && Timer % (int)(20 / player.GetAttackSpeed(DamageClass.Ranged)) == 0 && Timer * player.GetAttackSpeed(DamageClass.Ranged) < 180 * player.GetAttackSpeed(DamageClass.Ranged))
                    {
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 2;
                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(CustomSounds.Zap2 with { Pitch = 0.3f }, player.position);
                            SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch with { Volume = 0.6f }, Projectile.position);
                        }

                        offset = 10;
                        rotOffset = -0.02f;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            float shotOff = MathHelper.Max(shotOff2, 0);
                            for (int i = 0; i < Main.rand.Next(4, 6); i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation() + Main.rand.NextFloat(-shotOff, shotOff)), ProjectileType<XeniumElectrolaser_Beam>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, player.whoAmI, 1);
                            }
                        }
                        shotOff2 -= 0.06f;
                    }
                    if (Timer * player.GetAttackSpeed(DamageClass.Ranged) >= 180)
                        player.channel = false;
                    if (Timer % (int)(40 / player.GetAttackSpeed(DamageClass.Ranged)) == 0)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.ShieldActivate with { Volume = 0.6f, Pitch = Timer / 300 }, Projectile.position);
                        player.GetModPlayer<EnergyPlayer>().statEnergy -= 3;
                    }
                }
                else if (Timer * player.GetAttackSpeed(DamageClass.Ranged) >= 180 && player.GetModPlayer<EnergyPlayer>().statEnergy >= 2)
                {
                    if (Timer * player.GetAttackSpeed(DamageClass.Ranged) < 400)
                    {
                        if (player.GetModPlayer<EnergyPlayer>().statEnergy > 0)
                        {
                            if (!Main.dedServ)
                            {
                                if (Timer * player.GetAttackSpeed(DamageClass.Ranged) > 120)
                                {
                                    SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                                    SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
                                }
                                else
                                    SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch with { Volume = 0.6f }, Projectile.position);
                            }
                            offset = 20;
                            rotOffset = -0.08f;

                            if (Projectile.owner == Main.myPlayer)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), gunPos, RedeHelper.PolarVector(3, Projectile.velocity.ToRotation()), ProjectileType<XeniumElectrolaser_Beam>(), Projectile.damage * 5, Projectile.knockBack, player.whoAmI, 20);

                                player.velocity -= RedeHelper.PolarVector(3, (Main.MouseWorld - player.Center).ToRotation());
                            }
                            player.RedemptionScreen().ScreenShakeIntensity += 20;
                        }
                        Timer = 400;
                    }
                    else
                    {
                        if (Timer++ >= 420)
                            Projectile.Kill();
                    }
                }
                else
                    Projectile.Kill();
            }
            shake = MathHelper.Min(shake, 3f);
            offset = MathHelper.Clamp(offset, 0, 40);
            rotOffset = MathHelper.Clamp(rotOffset, -1, 0);
            if (alphaTimer++ > 1)
                Projectile.alpha = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (fakeKill)
                return false;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Vector2 v = RedeHelper.PolarVector(-26 + offset, Projectile.velocity.ToRotation());
            float timerMax = 180;
            int timerProgress = (int)(glow.Width * (glowTimer / timerMax));

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, timerProgress, glow.Height)), Projectile.GetAlpha(RedeColor.GreenPulse), Projectile.rotation + (rotOffset * Projectile.spriteDirection), drawOrigin + new Vector2(Projectile.spriteDirection == -1 ? -30 : -50, -12), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
