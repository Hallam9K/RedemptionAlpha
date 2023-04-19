using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Buffs.NPCBuffs;
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.BaseExtension;
using ParticleLibrary;
using Redemption.Particles;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class DragonCleaver_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/DragonCleaver";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Length = 40;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override bool? CanHitNPC(NPC target) => !target.friendly && Timer < 15 && Projectile.ai[0] != 0 ? null : false;

        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float SwingSpeed;
        private float glow;
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
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case -1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, player.position);
                        }
                        if (Timer < 15)
                            BlockProj();
                        if (Timer < 5 * SwingSpeed)
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
                            Projectile.Kill();
                        break;

                    case 0:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            speed = MathHelper.ToRadians(1);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                            vector = startVector * Length;
                        }
                        if (!player.channel)
                        {
                            Timer = 0;
                            Projectile.ai[0] = -1;
                            Projectile.netUpdate = true;
                        }
                        glow += 0.01f;
                        if (glow >= 0.4)
                        {
                            Timer = 0;
                            RedeDraw.SpawnRing(Projectile.Center, Color.OrangeRed, 0.2f, 0.85f, 4);
                            RedeDraw.SpawnRing(Projectile.Center, Color.OrangeRed, 0.2f);
                            SoundEngine.PlaySound(SoundID.Item88, Projectile.position);
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0] = 1;
                        }
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == (int)(7 * SwingSpeed))
                        {
                            if (hitFury is 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                    RedeHelper.PolarVector(15, (Projectile.Center - player.Center).ToRotation()),
                                    ModContent.ProjectileType<FireSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                        }
                        if (Timer < 15 * SwingSpeed)
                            BlockProj();
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(13, 17) * SwingSpeed)
                        {
                            if (--hitFury < 0)
                                hitFury = 0;

                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            if (hitFury > 0)
                            {
                                if (Main.MouseWorld.X < player.Center.X)
                                    player.direction = -1;
                                else
                                    player.direction = 1;

                                Rot = 0;
                                speed = MathHelper.ToRadians(1);
                                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() + ((MathHelper.PiOver2 + 0.2f) * Projectile.spriteDirection));
                                vector = startVector * Length;
                            }
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == (int)(5 * SwingSpeed))
                        {
                            if (hitFury is 0)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center,
                                    RedeHelper.PolarVector(15, (Projectile.Center - player.Center).ToRotation()),
                                    ModContent.ProjectileType<FireSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                        }
                        if (Timer < 15 * SwingSpeed)
                            BlockProj();
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.15f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.5f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= Main.rand.Next(13, 17) * SwingSpeed)
                        {
                            if (--hitFury < 0)
                                hitFury = 0;

                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            if (hitFury > 0)
                            {
                                if (Main.MouseWorld.X < player.Center.X)
                                    player.direction = -1;
                                else
                                    player.direction = 1;

                                Rot = 0;
                                speed = MathHelper.ToRadians(1);
                                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() - ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                                vector = startVector * Length;
                            }
                            Projectile.ai[0] = 1;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                }
            }
            if (Projectile.ai[0] > 0)
            {
                player.RedemptionScreen().Rumble(4, 1);
                if (Main.rand.NextBool(4))
                    ParticleManager.NewParticle(player.RandAreaInEntity(), RedeHelper.SpreadUp(1), new EmberParticle(), Color.OrangeRed, 1f);

                int dustIndex = Dust.NewDust(player.position, player.width, player.height, DustID.FlameBurst, Scale: 1f);
                Main.dust[dustIndex].velocity.Y = -5;
                Main.dust[dustIndex].velocity.X = 0;
                Main.dust[dustIndex].noGravity = true;
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = player.MountedCenter + vector;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        private void BlockProj()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 200 / 4)
                    continue;

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Fire) || target.ProjBlockBlacklist(true))
                    continue;

                DustHelper.DrawCircle(target.Center, DustID.Torch, 1, 4, 4, nogravity: true);
                target.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 4;
        }
        private int hitFury;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (Projectile.ai[0] > 0 && hitFury is 0)
                hitFury = 3;
            Projectile.localNPCImmunity[target.whoAmI] = Projectile.ai[0] > 0 ? 13 : 20;
            target.immune[Projectile.owner] = 0;

            Player player = Main.player[Projectile.owner];
            if (player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.OrangeRed * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}