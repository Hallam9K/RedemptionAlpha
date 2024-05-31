using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Spellsong_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[20];
        public Vector2[] oldPos = new Vector2[20];

        public override string Texture => "Redemption/Items/Weapons/HM/Melee/Spellsong";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spellsong, Core of the West");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
        private Player Player => Main.player[Projectile.owner];
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        private Vector2 startVector;
        private Vector2 vector;
        private Vector2 mouseOrig;
        public float Timer;
        public float progress;
        private float SwingSpeed;

        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Projectile.Center = armCenter + vector;

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;
            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        progress = Timer / (150 * SwingSpeed);
                        if (Timer++ == 0)
                        {
                            Projectile.scale *= Projectile.ai[2];
                            Length = 76 * Projectile.ai[2];
                            mouseOrig = Main.MouseWorld;
                            SoundEngine.PlaySound(SoundID.Item71, Player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                        }
                        if (Timer == (int)(25 * SwingSpeed))
                        {
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center,
                                RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                                ModContent.ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        if (progress < 1)
                        {
                            Rot = MathHelper.ToRadians(50 + 170f * MathF.Atan(3f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (progress >= 1)
                        {
                            if (!Player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            if (Main.MouseWorld.X < Player.Center.X)
                                Player.direction = -1;
                            else
                                Player.direction = 1;
                            startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - armCenter).ToRotation() - ((MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.spriteDirection));
                            mouseOrig = Main.MouseWorld;
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        progress = Timer / (150 * SwingSpeed);
                        if (Timer++ == (int)(25 * SwingSpeed))
                        {
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center,
                                RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                                ModContent.ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        if (progress < 1)
                        {
                            Rot = -MathHelper.ToRadians(190 + 170f * MathF.Atan(3f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (progress >= 1)
                        {
                            if (!Player.channel)
                            {
                                Projectile.Kill();
                                return;
                            }
                            mouseOrig = Main.MouseWorld;
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 2:
                        Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        progress = Timer / (260 * SwingSpeed);

                        if (Timer++ == (int)(15 * SwingSpeed))
                        {
                            SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center,
                                RedeHelper.PolarVector(55, (mouseOrig - armCenter).ToRotation()),
                                ModContent.ProjectileType<SpellsongSlash_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        if (progress < 0.6f)
                        {
                            Rot = MathHelper.ToRadians(200 + 170f * MathF.Atan(10f * MathHelper.Pi * (progress - 0.1f))) * Projectile.spriteDirection;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer == (int)(200 * SwingSpeed))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Player.Center - RedeHelper.PolarVector(4, Projectile.rotation),
                                RedeHelper.PolarVector(1, (Projectile.Center - armCenter).ToRotation()),
                                ModContent.ProjectileType<Spellsong_Beam>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                        }
                        if (progress >= 1)
                            Projectile.Kill();
                        break;
                }
            }

            if (Timer == 2)
            {
                opacity = 1;
                Projectile.alpha = 0;
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    oldDirVector.SetValue(vector, i);
            }
            opacity = MathHelper.Lerp(1, 0, progress);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = vector;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageSwordTrailPosition(20, armCenter, oldDirVector, ref directionVectorCache, ref positionCache, 0.9f);
                ManageTrail();
            }
        }
        private float opacity;
        public override bool? CanHitNPC(NPC target)
        {
            return (progress < 0.4f && Projectile.ai[0] == 2) || (progress <= 0.18f && Projectile.ai[0] < 2) ? null : false;
        }

        private Vector2[] oldDirVector = new Vector2[20];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        public Color baseColor = Color.LightPink;
        public Color endColor = Color.Purple;
        public Color edgeColor = Color.Purple;
        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, Projectile.oldPos.Length, new NoTip(),
            factor =>
            {
                float mult = factor;
                float delay = 0;
                if (mult < 0.99f)
                    delay = 1;
                return 40 * MathF.Pow(mult, 0.2f) * delay;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress) * opacity;
            });
            trail.Positions = positionCache.ToArray();
            trail.NextPosition = Projectile.Center;
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Effect effect = ModContent.Request<Effect>("Redemption/Effects/GlowTrailShader", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_4").Value);
            effect.Parameters["time"].SetValue(1);
            effect.Parameters["repeats"].SetValue(Player.direction);

            trail?.Render(effect);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrail();

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Vector2 v = RedeHelper.PolarVector(20, (Projectile.Center - armCenter).ToRotation());

            DrawSlash();
            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - v - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
        public void DrawSlash()
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D Swing = ModContent.Request<Texture2D>("Redemption/Textures/BladeSwing", AssetRequestMode.ImmediateLoad).Value;
            float radialOffset = Projectile.spriteDirection == -1 ? 5 * MathHelper.PiOver4 : -1 * MathHelper.PiOver4;
            float opacityAlt = Projectile.ai[0] == 2 ? 100f : 20f;
            float rotation = 0.5f * Rot + startVector.ToRotation() + radialOffset;
            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 4, -4);
            Rectangle rectangle = Swing.Frame(1, 4);
            Vector2 origin2 = rectangle.Size() / 2f;
            float opacity = 1 / (1 + opacityAlt * ((progress - 0.1f) * (progress - 0.1f)));

            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 0), RedeColor.NebColour * opacity * 0.5f, rotation + Player.direction * 0.1f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 1), RedeColor.NebColour * opacity * 0.3f, rotation + Player.direction * 0.01f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 2), RedeColor.NebColour * opacity * 0.7f, rotation + Player.direction * 0.1f, origin2, Projectile.scale * 1.32f, spriteEffects, 0f);
            Main.spriteBatch.Draw(Swing, armCenter - Main.screenPosition, Swing.Frame(1, 4, 0, 3), new Color(204, 153, 255) * 0.7f * opacity, rotation + Player.direction * 0.01f, origin2, Projectile.scale * 1.3f, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
    }
}
