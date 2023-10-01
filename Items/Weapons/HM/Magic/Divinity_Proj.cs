using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Redemption.BaseExtension;
using Redemption.Base;
using Terraria.GameContent;
using Redemption.Particles;
using ParticleLibrary;
using Redemption.Projectiles.Magic;
using Redemption.Effects;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Divinity_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Magic/Divinity";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Divinity");
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ownerHitCheck = true;
            Projectile.ignoreWater = true;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float num = MathHelper.ToRadians(0f);
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Projectile.spriteDirection == -1)
                num = MathHelper.ToRadians(180f);

            if (!player.channel)
                Projectile.Kill();

            if (Main.myPlayer == Projectile.owner)
            {
                float scaleFactor6 = 1f;
                if (player.inventory[player.selectedItem].shoot == Projectile.type)
                {
                    scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                }
                Vector2 vector13 = Main.MouseWorld - vector;
                vector13.Normalize();
                if (vector13.HasNaNs())
                {
                    vector13 = Vector2.UnitX * player.direction;
                }
                vector13 *= scaleFactor6;
                if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                    Projectile.netUpdate = true;

                Projectile.velocity = vector13;
                if (player.noItems || player.CCed || player.dead || !player.active)
                {
                    Projectile.Kill();
                }
                Projectile.netUpdate = true;
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(18, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (Projectile.localAI[0]++ == 0 && Projectile.owner == Main.myPlayer)
            {
                Projectile.alpha = 0;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-1, -8), ModContent.ProjectileType<Divinity_Sun>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Magic/Divinity_Glow").Value;
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, effects, 0);
            return false;
        }
    }
    public class Divinity_Sun : ModProjectile
    {
        public override string Texture => "Redemption/Textures/Sun";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.scale = 0.1f;
            Projectile.extraUpdates = 1;
        }
        private float godrayFade;
        public Vector2 mark;
        private int charged;
        public override void AI()
        {
            Projectile.width = Projectile.height = (int)(40 * Projectile.scale);
            Projectile.velocity *= 0.95f;
            Projectile.timeLeft = 10;
            Projectile staff = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[staff.owner];
            if (Projectile.owner == Main.myPlayer)
            {
                switch (Projectile.ai[1])
                {
                    case 0:
                        godrayFade += 0.02f;
                        Projectile.scale += 0.03f;
                        if (Projectile.alpha > 0)
                            Projectile.alpha -= 8;
                        if (Projectile.scale >= 1)
                        {
                            Projectile.localAI[1] = 30;
                            Projectile.scale = 1;
                            if (player.channel)
                                Projectile.ai[1] = 1;
                            else
                                Projectile.Kill();
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        if (player.DistanceSQ(Projectile.Center) > 1800 * 1800)
                        {
                            staff.active = false;
                            Projectile.Kill();
                            Projectile.netUpdate = true;
                        }
                        if (Projectile.DistanceSQ(player.Center) > 300 * 300)
                            Projectile.Move(player.Center - new Vector2(0, 100), 2, 20);
                        if (Projectile.localAI[0]++ % 14 == 0 && Projectile.scale < 3)
                        {
                            int mana = player.inventory[player.selectedItem].mana;
                            if (BasePlayer.ReduceMana(player, mana / 10))
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), staff.Center, RedeHelper.PolarVector(Main.rand.Next(5, 9), (Main.MouseWorld - player.Center).ToRotation() + (MathHelper.PiOver2 * (Main.rand.NextBool() ? -1 : 1))), ModContent.ProjectileType<Divinity_Ball>(), Projectile.damage / 2, 0, player.whoAmI, Projectile.whoAmI);
                            }
                        }
                        if (Projectile.scale >= 1.1f && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<Divinity_Crosshair>()))
                        {
                            if (charged == 0)
                            {
                                RedeDraw.SpawnExplosion(Projectile.Center, Color.White, shakeAmount: 0, scale: 2, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                                SoundEngine.PlaySound(CustomSounds.NebSound2 with { Pitch = .2f, Volume = .5f }, Projectile.position);
                                charged = 1;
                            }
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Divinity_Crosshair>(), 0, 0, player.whoAmI, Projectile.whoAmI);
                        }
                        if (Projectile.scale >= 3 && charged < 2)
                        {
                            RedeDraw.SpawnExplosion(Projectile.Center, Color.White, shakeAmount: 0, scale: 5, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                            SoundEngine.PlaySound(CustomSounds.NebSound2 with { Volume = .5f }, Projectile.position);
                            charged = 2;
                        }
                        if (!player.channel)
                        {
                            if (Projectile.scale >= 1.1f)
                            {
                                player.RedemptionScreen().ScreenShakeIntensity += 10 * Projectile.scale;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                                int pieCut = 32;
                                for (int m = 0; m < pieCut; m++)
                                {
                                    int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.OrangeTorch, 0f, 0f, 100, Color.White, 4);
                                    Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(14f, 0f), m / (float)pieCut * 6.28f);
                                    Main.dust[dustID].noGravity = true;
                                }
                                mark = Main.MouseWorld;
                                Projectile.ai[1] = 2;
                            }
                            else
                                Projectile.Kill();
                        }
                        break;
                    case 2:
                        if (Projectile.DistanceSQ(mark) < 10 * 10)
                            Projectile.Kill();
                        else
                            Projectile.Move(mark, 34, 1);
                        break;
                }
            }
            Projectile.scale = MathHelper.Min(Projectile.scale, 3);
        }
        public override void OnKill(int timeLeft)
        {
            Projectile staff = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[staff.owner];
            player.RedemptionScreen().ScreenShakeIntensity += 20 * (Projectile.scale * 2);
            for (int i = 0; i < 80; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new EmberParticle(), Color.White, 3 * Projectile.scale, 0, 2);
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new EmberParticle(), Color.White, 1, 0);
            SoundEngine.PlaySound(SoundID.Item100 with { Volume = 1 * Projectile.scale, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, Projectile.position);

            int boomOrigin = (int)(120 * Projectile.scale);
            Rectangle boom = new((int)Projectile.Center.X - boomOrigin, (int)Projectile.Center.Y - boomOrigin, boomOrigin * 2, boomOrigin * 2);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                    continue;

                target.immune[Projectile.whoAmI] = 20;
                int hitDirection = target.RightOfDir(Projectile);
                BaseAI.DamageNPC(target, (int)(Projectile.damage * (Projectile.scale * 1.5f)), 7, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                BaseAI.DamageNPC(target, (int)(Projectile.damage * (Projectile.scale * 1.25f)), 4, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                target.AddBuff(BuffID.OnFire3, 600);
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, Scale: 2);
                Main.dust[dust].velocity *= 6;
                Main.dust[dust].noGravity = true;
            }
            if (Projectile.owner == Main.myPlayer && Projectile.scale >= 1.1f)
            {
                for (int i = 0; i < Main.rand.Next(8, 14) * Projectile.scale; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.SpreadUp(12), ProjectileID.MolotovFire3, Projectile.damage / 2, 1, Main.myPlayer);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Projectile.scale;
            if (Projectile.ai[1] != 2)
                modifiers.FinalDamage /= 4;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D aura = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawOrigin2 = new(aura.Width / 2, aura.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.9f, 1.1f, 0.9f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.1f, 0.9f, 1.1f);
            float colourScale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.4f, 0.6f, 0.4f);

            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = Projectile.scale * (1 + fluctuate);

                Color godrayColor = Color.Lerp(new Color(252, 243, 201), Color.White * Projectile.Opacity, 0.5f);
                godrayColor.A = 0;
                RedeDraw.DrawGodrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, godrayColor * godrayFade, 80 * modifiedScale * Projectile.Opacity, 10 * modifiedScale * Projectile.Opacity, 16);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), -Projectile.rotation, drawOrigin, Projectile.scale * scale2, effects, 0);

            Main.EntitySpriteDraw(aura, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White) * colourScale, Projectile.rotation, drawOrigin2, Projectile.scale * scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
    public class Divinity_Ball : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteOrb";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Divinity Charge");
            ElementID.ProjFire[Type] = true;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = Main.rand.NextFloat(0.4f, 0.6f);
        }

        private readonly int NUMPOINTS = 20;
        public Color baseColor = new(252, 243, 201);
        public Color endColor = Color.White;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 2f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 400);
        }
        public override void AI()
        {
            Projectile sun = Main.projectile[(int)Projectile.ai[0]];
            if (!sun.active || sun.type != ModContent.ProjectileType<Divinity_Sun>())
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Move(sun.Center, 35, 40);
            if (Projectile.Hitbox.Intersects(sun.Hitbox))
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
                sun.scale += 0.02f;
                Projectile.Kill();
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/GlowTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(252, 243, 201), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Sandnado, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
