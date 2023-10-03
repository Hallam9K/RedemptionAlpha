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
using Redemption.Buffs.NPCBuffs;
using Terraria.GameContent;
using Redemption.Particles;
using ReLogic.Utilities;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class GlobalDischarge_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Magic/GlobalDischarge";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Global Discharge");
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 50;
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
                num = MathHelper.ToRadians(90f);

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
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(30, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + num + MathHelper.PiOver4;
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
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + Vector2.Normalize(Projectile.velocity) * 58f, RedeHelper.PolarVector(18, (Main.MouseWorld - player.Center).ToRotation()), ModContent.ProjectileType<GlobalDischarge_Sphere>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Magic/GlobalDischarge_Glow").Value;
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, effects, 0);
            return false;
        }
    }
    public class GlobalDischarge_Sphere : ModProjectile
    {
        public override string Texture => "Redemption/Textures/StaticBall";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Global Discharge");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjThunder[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 164;
            Projectile.height = 164;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.scale = 1f;
        }
        private float godrayFade;
        private float speed = 6;
        private SlotId loop;
        private ActiveSound sound;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.01f;
            Projectile.timeLeft = 10;
            Projectile staff = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[staff.owner];
            if ((!staff.active || staff.type != ModContent.ProjectileType<GlobalDischarge_Proj>() || player.DistanceSQ(Projectile.Center) > 1400 * 1400))
            {
                if (Projectile.ai[1] > 0)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Spark1, Projectile.position);

                    staff.active = false;
                    Projectile.ai[1] = -2;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Frost);
                dust.velocity = -(player.Center + Vector2.Normalize(staff.velocity) * 58f).DirectionTo(dust.position) * 10;
                dust.noGravity = true;
                int steps = (int)Projectile.Distance(player.Center + Vector2.Normalize(staff.velocity) * 58f) / 8;
                for (int i = 0; i < steps; i++)
                {
                    if (Main.rand.NextBool(4))
                    {
                        Dust dust2 = Dust.NewDustDirect(Vector2.Lerp(Projectile.Center, player.Center + Vector2.Normalize(staff.velocity) * 58f, (float)i / steps), 2, 2, DustID.Frost);
                        dust2.velocity = (player.Center + Vector2.Normalize(staff.velocity) * 58f).DirectionTo(dust2.position) * 2;
                        dust2.noGravity = true;
                    }
                }
            }
            if (Main.rand.NextBool(10))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90 * Projectile.scale, RedeHelper.RandomRotation()), 1.5f, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90 * Projectile.scale, RedeHelper.RandomRotation()), 1.5f, 20, 0.1f);
            }
            if (Projectile.ai[1] < 0 && Main.rand.NextBool(2))
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(180 * Projectile.scale, RedeHelper.RandomRotation()), 1.5f, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(180 * Projectile.scale, RedeHelper.RandomRotation()), 1.5f, 20, 0.1f);
            }

            if (Projectile.owner == Main.myPlayer)
            {
                switch (Projectile.ai[1])
                {
                    case -2:
                        if (sound != null)
                        {
                            sound.Stop();
                            loop = SlotId.Invalid;
                        }

                        godrayFade += 0.04f;
                        Projectile.scale += 0.01f;
                        Projectile.alpha -= 1;
                        Projectile.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                        if (godrayFade >= 1f)
                        {
                            RedeDraw.SpawnExplosion(Projectile.Center, Color.White, DustID.Electric, 28, 40);
                            Projectile.Kill();
                        }
                        break;
                    case -1:
                        if (sound != null)
                        {
                            sound.Stop();
                            loop = SlotId.Invalid;
                        }

                        godrayFade += 0.02f;
                        Projectile.scale += 0.01f;
                        Projectile.alpha -= 1;
                        Projectile.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                        if (godrayFade >= 1.2f)
                        {
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                NPC npc = Main.npc[i];
                                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                                    continue;

                                if (Projectile.DistanceSQ(npc.Center) > 600 * 600)
                                    continue;

                                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, npc.Center, 2f, 20, 0.05f);
                                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, npc.Center, 2f, 20, 0.05f);
                                int hitDirection = npc.RightOfDir(Projectile);
                                BaseAI.DamageNPC(npc, Projectile.damage * 2, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                            }
                            RedeDraw.SpawnExplosion(Projectile.Center, Color.White, DustID.Electric, 60, 40, 2, 5);
                            Projectile.Kill();
                        }
                        break;
                    case 0:
                        if (Projectile.localAI[0] == 0)
                        {
                            if (sound == null)
                                loop = SoundEngine.PlaySound(CustomSounds.ElectricLoop, Projectile.position);
                            Projectile.scale = .1f;
                            Projectile.localAI[0] = 1;
                        }
                        Projectile.scale += 0.03f;
                        Projectile.velocity *= 0.95f;
                        if (Projectile.alpha > 0)
                            Projectile.alpha -= 8;
                        if (Projectile.scale >= 1)
                        {
                            Projectile.localAI[0] = 0;
                            Projectile.localAI[1] = 30;
                            Projectile.scale = 1;
                            if (player.channel)
                                Projectile.ai[1] = 1;
                            else
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.Spark1, Projectile.position);
                                Projectile.ai[1] = -1;
                            }
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        if (sound == null)
                            loop = SoundEngine.PlaySound(CustomSounds.ElectricLoop, Projectile.position);

                        if (Projectile.DistanceSQ(staff.Center + RedeHelper.PolarVector(140, staff.velocity.ToRotation())) >= 300 * 300)
                        {
                            Projectile.Move(staff.Center + RedeHelper.PolarVector(80, staff.velocity.ToRotation()), speed, 6);
                            speed *= 1.04f;
                        }
                        speed *= .98f;
                        speed = MathHelper.Clamp(speed, 4, 34);
                        break;
                }
            }
            SoundEngine.TryGetActiveSound(loop, out sound);
            if (sound != null)
            {
                sound.Position = Projectile.position;
                sound.Volume = Projectile.velocity.Length() / 24;
                sound.Volume = MathHelper.Clamp(sound.Volume, 0, 1);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= (Projectile.velocity.Length() / 10) + .4f;
        }
        public override void OnKill(int timeLeft)
        {
            if (sound != null)
            {
                sound.Stop();
                loop = SlotId.Invalid;
            }
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 360);
            if (Projectile.ai[1] != 1)
                return;
            float f = (Projectile.velocity.Length() / 28) + 1;
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.Zap2 with { Volume = f * .5f }, Projectile.position);
            if (target.knockBackResist != 0)
                target.velocity += Projectile.velocity * target.knockBackResist;

            for (int i = 0; i < 4; i++)
            {
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90 * Projectile.scale * f, RedeHelper.RandomRotation()), 1.5f * f, 20, 0.1f);
                DustHelper.DrawParticleElectricity<LightningParticle>(Projectile.Center, Projectile.Center + RedeHelper.PolarVector(90 * Projectile.scale * f, RedeHelper.RandomRotation()), 1.5f * f, 20, 0.1f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D orb = ModContent.Request<Texture2D>("Redemption/Textures/CultistOrb").Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.9f, 1.1f, 0.9f);
            float scale2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.1f, 0.9f, 1.1f);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, Color.LightCyan, Color.Cyan, Color.LightCyan);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            int heightO = orb.Height / 4;
            int yO = heightO * Projectile.frame;
            Rectangle rectO = new(0, yO, orb.Width, heightO);
            Vector2 drawOriginO = new(orb.Width / 2, heightO / 2);
            Main.EntitySpriteDraw(orb, Projectile.Center - Main.screenPosition, new Rectangle?(rectO), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOriginO, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color) * 0.9f, Projectile.rotation, drawOrigin, Projectile.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color) * 0.9f, -Projectile.rotation, drawOrigin, Projectile.scale * scale2, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = Projectile.scale * (1 + fluctuate);

                Color godrayColor = Color.Lerp(new Color(255, 255, 255), Color.White * Projectile.Opacity, 0.5f);
                godrayColor.A = 0;
                RedeDraw.DrawGodrays(Main.spriteBatch, Projectile.Center - Main.screenPosition, godrayColor * godrayFade, 120 * modifiedScale * Projectile.Opacity, 35 * modifiedScale * Projectile.Opacity, 16);
            }
            return false;
        }
    }
}
