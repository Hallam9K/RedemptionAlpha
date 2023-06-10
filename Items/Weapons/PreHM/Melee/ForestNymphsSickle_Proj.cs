using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Melee;
using Terraria.ModLoader;
using System;
using Redemption.Projectiles.Magic;
using Terraria.DataStructures;
using Redemption.Base;

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
            Length = 66;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Timer < 15 && Projectile.ai[0] != 2 ? null : false;

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
        private float speed;
        private float SwingSpeed;
        private Vector2 mouseOrig;
        private float glow;
        private bool lifeDrained;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Projectile.spriteDirection = player.direction;
            if (Projectile.ai[0] < 2)
            {
                if (Projectile.spriteDirection == 1)
                    Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
                else
                    Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
                glow += 0.03f;
            }
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Timer++ == 0)
                        {
                            mouseOrig = Main.MouseWorld;
                            speed = MathHelper.ToRadians(1);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                            vector = startVector * Length;
                            SoundEngine.PlaySound(SoundID.Item71, player.position);
                        }
                        if (Timer == (int)(4 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                RedeHelper.PolarVector(14, (mouseOrig - player.Center).ToRotation()),
                                ModContent.ProjectileType<ForestSickle_Proj>(), (int)(Projectile.damage * .75f), Projectile.knockBack / 2, Projectile.owner);
                        }
                        if (Timer < 6 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.7f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 25 * SwingSpeed)
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
                            Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(1);
                            Rot = MathHelper.ToRadians(2);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() + ((MathHelper.PiOver2 + 0.6f) * player.direction));
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
                                RedeHelper.PolarVector(14, (mouseOrig - player.Center).ToRotation()),
                                ModContent.ProjectileType<ForestSickle_Proj>(), (int)(Projectile.damage * .75f), Projectile.knockBack / 2, Projectile.owner);
                        }
                        if (Timer < 6 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.7f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 25 * SwingSpeed)
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
                            if (Main.rand.NextBool(5) && player.ownedProjectileCounts[ModContent.ProjectileType<NaturePixie_Magic>()] < 4)
                            {
                                if (BasePlayer.ReduceMana(player, 8))
                                {
                                    SoundEngine.PlaySound(SoundID.Item101, Projectile.position);
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloat(-3, 3), -Main.rand.NextFloat(4, 8)), ModContent.ProjectileType<NaturePixie_Magic>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
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

            Projectile.Center = player.MountedCenter + vector;
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
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
            if (!lifeDrained)
            {
                Player player = Main.player[Projectile.owner];
                player.statLife += (int)Math.Floor((double)damageDone / 20);
                player.HealEffect((int)Math.Floor((double)damageDone / 20));
                lifeDrained = true;
            }
            Projectile.localNPCImmunity[target.whoAmI] = 20;
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
            Vector2 v = RedeHelper.PolarVector(20, (Projectile.Center - player.Center).ToRotation());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - v - Main.screenPosition + trialOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.LimeGreen * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * Projectile.Opacity * glow, oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}