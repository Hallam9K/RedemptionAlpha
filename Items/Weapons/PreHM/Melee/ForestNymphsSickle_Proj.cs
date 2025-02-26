using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Projectiles.Magic;
using Redemption.Projectiles.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForestNymphsSickle_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/ForestNymphsSickle";

        public float[] oldrot = new float[7];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forest Nymph's Sickle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjNature[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;

            Length = 66;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && progress < 0.25 && Projectile.ai[0] != 2 ? null : false;

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 2)
                Projectile.DamageType = DamageClass.Magic;
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float SwingSpeed;
        private Vector2 mouseOrig;
        private float glow;
        private bool lifeDrained;
        public int pauseTimer;
        public float progress;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.Center = playerCenter + vector;

            progress = Timer / (25 * SwingSpeed);

            Projectile.spriteDirection = player.direction;
            if (Projectile.ai[0] < 2)
            {
                if (Projectile.spriteDirection == 1)
                    Projectile.rotation = (Projectile.Center - playerCenter).ToRotation() + MathHelper.PiOver4;
                else
                    Projectile.rotation = (Projectile.Center - playerCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
                glow += 0.03f;
            }
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (playerCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Main.myPlayer == Projectile.owner && --pauseTimer <= 0)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Timer++ == 0)
                        {
                            mouseOrig = Main.MouseWorld;
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                            vector = startVector * Length;
                            SoundEngine.PlaySound(SoundID.Item71, player.position);
                        }
                        if (Timer == (int)(4 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(14, (mouseOrig - playerCenter).ToRotation()),
                                ProjectileType<ForestSickle_Proj>(), (int)(Projectile.damage * .75f), Projectile.knockBack / 2, Projectile.owner);
                        }
                        if (progress < 0.24f)
                        {
                            Rot = MathHelper.ToRadians(750 * progress) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot = MathHelper.ToRadians(750 * (0.333f - MathF.Pow(0.00005f, progress))) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (progress >= 1)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (Main.MouseWorld.X < player.Center.X)
                                player.direction = -1;
                            else
                                player.direction = 1;
                            Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - playerCenter).ToRotation());
                            Projectile.alpha = 255;
                            Rot = MathHelper.ToRadians(2);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - playerCenter).ToRotation() + ((MathHelper.PiOver2 + 0.6f) * player.direction));
                            vector = startVector * Length;
                            mouseOrig = Main.MouseWorld;
                            lifeDrained = false;
                            Projectile.ai[0]++;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            glow = 0;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        if (Timer++ == (int)(4 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(14, (mouseOrig - playerCenter).ToRotation()),
                                ProjectileType<ForestSickle_Proj>(), (int)(Projectile.damage * .75f), Projectile.knockBack / 2, Projectile.owner);
                        }
                        if (progress < 0.24f)
                        {
                            Rot = -MathHelper.ToRadians(750 * progress) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot = -MathHelper.ToRadians(750 * (0.333f - MathF.Pow(0.00005f, progress))) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (progress >= 1)
                            Projectile.Kill();
                        break;
                    case 2:
                        if (Timer++ == 0)
                            vector = new Vector2(6 * player.direction, -20);

                        int dustIndex = Dust.NewDust(new Vector2(player.position.X, player.Bottom.Y - 2), player.width, 2, DustID.DryadsWard);
                        Main.dust[dustIndex].velocity.Y = -Main.rand.Next(3, 7);
                        Main.dust[dustIndex].velocity.X = 0;
                        Main.dust[dustIndex].noGravity = true;
                        if (glow < 1)
                            glow += .03f;
                        Projectile.rotation = ((float)Math.Sin(Timer / 20) / 6) + (player.direction == -1 ? .3f : -.3f);
                        if (!lifeDrained)
                        {
                            startVector.Y += 0.1f;
                            if (startVector.Y > 1.2f)
                                lifeDrained = true;
                        }
                        else if (lifeDrained)
                        {
                            startVector.Y -= 0.1f;
                            if (startVector.Y < -1.2f)
                                lifeDrained = false;
                        }
                        vector = new Vector2(6 * player.direction, -20) + startVector;
                        if (Timer >= 30)
                        {
                            if (Main.rand.NextBool(5) && player.ownedProjectileCounts[ProjectileType<NaturePixie_Magic>()] < 4)
                            {
                                if (BasePlayer.ReduceMana(player, 8))
                                {
                                    SoundEngine.PlaySound(SoundID.Item101, Projectile.position);
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloat(-3, 3), -Main.rand.NextFloat(4, 8)), ProjectileType<NaturePixie_Magic>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                                }
                            }
                        }
                        if (!player.channel)
                            Projectile.Kill();
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dark.Contains(target.type))
                modifiers.FinalDamage *= 1.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(CustomSounds.Slice4 with { Volume = .7f, Pitch = .2f }, Projectile.position);
            player.RedemptionScreen().ScreenShakeIntensity += 4;
            pauseTimer = 4;
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 directionTo = target.DirectionTo(playerCenter);
            for (int i = 0; i < 8; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 5 + new Vector2(0, 35) + player.velocity, DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(4f, 5f) + (player.velocity / 2), 0, Color.LimeGreen * .8f, 1.6f);

            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (!lifeDrained)
            {
                player.statLife += (int)Math.Floor((double)damageDone / 20);
                player.HealEffect((int)Math.Floor((double)damageDone / 20));
                lifeDrained = true;
            }
            Projectile.localNPCImmunity[target.whoAmI] = 10;
            target.immune[Projectile.owner] = 0;

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.DryadsWardDebuff, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 trialOrigin = new(texture.Width / 2f - 8, Projectile.height / 2f);
            if (Projectile.ai[0] == 2)
                origin = new(texture.Width / 2f - (30 * player.direction), texture.Height / 2f + 34);

            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 v = RedeHelper.PolarVector(20, (Projectile.Center - playerCenter).ToRotation());

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - v - Main.screenPosition + trialOrigin;
                Color color = Color.LimeGreen * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * Projectile.Opacity * glow, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}