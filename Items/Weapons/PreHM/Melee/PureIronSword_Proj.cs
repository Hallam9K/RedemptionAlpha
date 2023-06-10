using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class PureIronSword_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Melee/PureIronSword";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Sword");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjIce[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Length = 52;
            Rot = MathHelper.ToRadians(2);
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 2 && Timer > 9)
                return false;
            if (Projectile.ai[0] == 1 && Timer > 8)
                return false;
            return !target.friendly && Projectile.ai[0] != 0 ? null : false;
        }

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
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;


            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        speed = MathHelper.ToRadians(6);
                        startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() + ((MathHelper.PiOver2 + 0.6f) * Projectile.spriteDirection));
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                        vector = startVector * Length;
                        Projectile.ai[0] = 2;
                        Projectile.netUpdate = true;
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        BlockProj();
                        if (Timer++ < 2 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.35f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.6f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 18 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (glow >= 0.8f)
                            {
                                RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                                RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f);
                                SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                                Projectile.ai[0] = 3;
                            }
                            else
                            {
                                if (Main.MouseWorld.X < player.Center.X)
                                    player.direction = -1;
                                else
                                    player.direction = 1;
                                Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());
                                Projectile.alpha = 255;
                                speed = MathHelper.ToRadians(6);
                                Rot = MathHelper.ToRadians(2);
                                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() + ((MathHelper.PiOver2 + 0.6f) * player.direction));
                                vector = startVector * Length;
                                Projectile.ai[0]++;
                            }
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        BlockProj();
                        if (Timer++ < 3 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.2f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 15 * SwingSpeed)
                        {
                            if (!player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (glow >= 0.8f)
                            {
                                RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f, 0.85f, 4);
                                RedeDraw.SpawnRing(Projectile.Center, Color.LightCyan, 0.2f);
                                SoundEngine.PlaySound(SoundID.Item30, Projectile.position);
                                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<ArcticWind_Proj>(), 0, 0, Projectile.owner);
                                Projectile.ai[0] = 3;
                            }
                            else
                            {
                                Projectile.alpha = 255;
                                speed = MathHelper.ToRadians(6);

                                Projectile.ai[0] = 1;
                            }

                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 3:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        BlockProj();
                        if (Timer++ < 2 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.25f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer > 30 * SwingSpeed)
                            speed *= 0.9f;
                        if (Timer >= 44 * SwingSpeed)
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
                            glow = 0;
                            Projectile.velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());
                            Projectile.alpha = 255;
                            speed = MathHelper.ToRadians(6);
                            Rot = MathHelper.ToRadians(2);
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation() + ((MathHelper.PiOver2 + 0.6f) * player.direction));
                            vector = startVector * Length;

                            Projectile.ai[0] = 2;
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.position);
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;
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

                if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Ice) || target.ProjBlockBlacklist(true))
                    continue;

                DustHelper.DrawCircle(target.Center, DustID.IceTorch, 1, 4, 4, nogravity: true);
                target.Kill();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 3)
                modifiers.FinalDamage *= 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Projectile.localNPCImmunity[target.whoAmI] = 11;
            target.immune[Projectile.owner] = 0;

            if (glow < 0.8f)
                glow += 0.2f;
            Player player = Main.player[Projectile.owner];
            if (player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.2f, 1.1f, 1.2f);
            Vector2 v = RedeHelper.PolarVector(10, (Projectile.Center - player.Center).ToRotation());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.LightBlue * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos - v, null, color * glow, oldrot[k], origin, Projectile.scale * scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}