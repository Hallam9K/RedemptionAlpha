using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Particles;
using Redemption.Projectiles.Melee;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PZGauntlet_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjPoison[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        Vector2 dirVec;
        Vector2 launchVec;
        float timer;
        float progress;
        float OpacityTimer;
        bool launch;
        bool onHit;
        NPC aimmed;
        Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 3, -3);
            if (Main.myPlayer == Projectile.owner)
            {
                if (!launch)
                {
                    dirVec = armCenter.DirectionTo(Main.MouseWorld);
                    Player.direction = armCenter.X < Main.MouseWorld.X ? 1 : -1;
                    Projectile.spriteDirection = Player.direction;
                    Projectile.rotation = dirVec.ToRotation() + MathHelper.PiOver2;
                    Projectile.Center = armCenter + dirVec * (15 - 8 * progress);
                    Charge();
                }
                else
                {
                    Projectile.Center = armCenter + dirVec * 15;
                    Launch();
                }
            }
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi);
        }
        public void Charge()
        {
            timer++;
            Player.ChangeDir(Projectile.spriteDirection);
            progress = MathHelper.Clamp(timer / (SetSwingSpeed(1) * 50), 0, 1);

            if (progress <= 0.5f)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(50, 75);
                Vector2 velocity = spawnPos.DirectionTo(Projectile.Center);
                MakeDust(spawnPos, velocity * 100);
            }
            if (progress >= 1 && !Player.channel)
            {
                launchVec = Player.Center.DirectionTo(Main.MouseWorld);
                dirVec = launchVec;
                launch = true;
                timer = 0;
            }
        }
        public void Launch()
        {
            OpacityTimer++;
            Player.GetModPlayer<RedePlayer>().fallSpeedIncrease += 50;
            if (timer++ == 0)
            {
                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            }
            if (timer <= 15)
            {
                MakeDustTrail();
                MakeDust(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity);
                Player.velocity = launchVec * 40;
                Projectile.friendly = true;
                Player.Redemption().contactImmune = true;
            }
            else if (timer <= 25)
            {
                Player.Redemption().contactImmune = true;
            }
            else
            {
                Player.velocity *= 0.5f;
            }
            if (timer >= 30)
                Projectile.Kill();

            if (onHit && aimmed.active)
            {
                if (timer < 25)
                    Player.Redemption().contactImmune = true;

                Vector2 spawnPos = aimmed.Center + Main.rand.NextVector2CircularEdge(200, 200);
                Vector2 velocity = spawnPos.DirectionTo(aimmed.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, velocity * 24, ProjectileType<PZGauntlet_Proj2>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Player.whoAmI);
            }
        }
        public void MakeDustTrail()
        {
            Vector2 position = Projectile.Center + dirVec * 10f;
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, 16, 16, DustID.Clentaminator_Green)];
            dust.position = position;
            dust.velocity = dirVec.RotatedBy(1.57) * 5f;
            dust.position += dirVec.RotatedBy(1.57);
            dust.fadeIn = 1f;
            dust.noGravity = true;
            dust = Main.dust[Dust.NewDust(Projectile.position, 16, 16, DustID.Clentaminator_Green)];
            dust.position = position;
            dust.velocity = dirVec.RotatedBy(-1.57) * 5f;
            dust.position += dirVec.RotatedBy(-1.57);
            dust.fadeIn = 1f;
            dust.noGravity = true;
        }
        public void MakeDust(Vector2 drawPos, Vector2 velocity)
        {
            if (launch)
            {
                if (timer % 3 == 2)
                    RedeParticleManager.CreateSpeedParticle(drawPos, velocity, .75f, Color.ForestGreen.WithAlpha(0));
            }
            else
            {
                if (timer % 2 == 1)
                    RedeParticleManager.CreateChargeParticle(3, Color.ForestGreen, Player.whoAmI, Projectile.whoAmI, velocity, 25, 40);
            }
        }
        public override bool? CanHitNPC(NPC target) => launch && Player.velocity.Length() > 0.5f ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!onHit)
            {
                timer = 15;
                aimmed = target;
                onHit = true;

                Player.AddImmuneTime(-1, 40);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<PZGauntlet_Explosion>(), 0, 0, Player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 2f }, Projectile.position);
            if (target.knockBackResist > 0)
                target.AddBuff(BuffType<StunnedDebuff>(), 15);

            Vector2 drawPos = Vector2.Lerp(Projectile.Center, target.Center, 0.5f);
            RedeParticleManager.CreateLaserParticle(drawPos, dirVec, 4f, Color.LightGreen, 4);

            for (int i = 0; i < 12; i++)
            {
                float randomSpeed = Main.rand.NextFloat(20, 40);
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 4, 4, DustID.Clentaminator_Green, Scale: 2)];
                dust.noGravity = true;
                dust.velocity = dirVec.RotatedByRandom(0.8f) * randomSpeed;
            }
        }
        public void DrawSpearEffect()
        {
            SpriteEffects dir = SpriteEffects.None;
            float angle = dirVec.ToRotation() + MathHelper.PiOver4;
            if (Player.direction > 0)
            {
                dir = SpriteEffects.FlipHorizontally;
                angle -= (float)Math.PI / 2f;
            }
            if (Player.gravDir == -1f)
            {
                if (Projectile.direction == 1)
                {
                    dir = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                    angle -= (float)Math.PI / 2f;
                }
                else if (Projectile.direction == -1)
                {
                    dir = SpriteEffects.FlipVertically;
                    angle += (float)Math.PI / 2f;
                }
            }

            Texture2D glow = TextureAssets.Extra[98].Value;
            Vector2 origin = glow.Size() / 2f;
            Color color = Color.LightGreen with { A = 0 };
            float rotation = angle - MathHelper.Pi / 4f * Projectile.spriteDirection;
            if (Player.gravDir < 0f)
                rotation -= MathHelper.Pi / 2f * Projectile.spriteDirection;

            Vector2 playerCenter = Player.RotatedRelativePoint(Player.MountedCenter, reverseRotation: false, addGfxOffY: true);
            Vector2 endPos = Projectile.Center;

            float progress = (timer - 16) / 14;
            float x = (progress - 0.1f) * 4f;
            float modifiedProgress = 1 / (1 + x * x);
            for (float num7 = 0.1f; num7 <= 0.8f; num7 += 0.1f)
            {
                Vector2 drawPos = Vector2.Lerp(playerCenter, endPos, num7);
                Vector2 scale = new Vector2(num7 * 0.7f, 1f);
                Main.EntitySpriteDraw(glow, drawPos - Main.screenPosition, null, color * modifiedProgress, rotation, origin, scale * modifiedProgress * 2, dir);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 dirOffeset = dirVec.SafeNormalize(Vector2.One) * Projectile.scale;
            Vector2 drawPos = Projectile.Center + dirOffeset * 0 + dirOffeset.RotatedBy(MathHelper.PiOver2) * 2 * Player.direction;

            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            if (onHit)
                DrawSpearEffect();
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D flare = Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Texture2D trail = Request<Texture2D>("Redemption/Textures/Ray", AssetRequestMode.ImmediateLoad).Value;

            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 origin2 = new(Projectile.width / 4, 0);

            Color colour = Color.Lerp(Color.White, Color.DarkOliveGreen, 1f / OpacityTimer * 10f) * (1f / OpacityTimer * 10f);

            float opacity = MathHelper.Lerp(2, 0, OpacityTimer / 48);
            float scale = MathHelper.Lerp(1, 0, OpacityTimer / 48);

            Vector2 dirOffeset = dirVec.SafeNormalize(Vector2.One) * Projectile.scale;
            Vector2 drawPos = Projectile.Center + dirOffeset * 0 + dirOffeset.RotatedBy(MathHelper.PiOver2) * 2 * Player.direction;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            if (launch)
            {
                Main.EntitySpriteDraw(flare, drawPos - Main.screenPosition, new Rectangle?(rect), colour * 0.5f * opacity, Projectile.rotation, origin, 0.3f * scale, 0, 0);
                Main.EntitySpriteDraw(flare, drawPos - Main.screenPosition, new Rectangle?(rect), colour * 0.25f * opacity, Projectile.rotation, origin, 0.4f * scale, 0, 0);
                Main.EntitySpriteDraw(trail, drawPos - Main.screenPosition, new Rectangle?(rect), colour * 0.5f * opacity, Projectile.rotation, origin2, 1.6f * scale, 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
    }
    public class PZGauntlet_Explosion : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        private float GlowTimer;
        private bool Glow;
        private float Rot;
        public Color color = Color.LightGreen;
        public float dustScale = 1f;
        public float scale = 1f;
        public Texture2D texture;
        public override void AI()
        {
            if (Glow)
            {
                GlowTimer += 3;
                if (GlowTimer > 60)
                {
                    Glow = false;
                    GlowTimer = 0;
                }
            }
            if (Projectile.localAI[0]++ == 0)
            {
                Glow = true;
                Projectile.alpha = 255;
                Rot = Main.rand.NextFloat(MathHelper.PiOver4, 3 * MathHelper.PiOver4);
                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, DustType<GlowDust>(), 0f, 0f, Scale: .5f);
                    Main.dust[dust].velocity *= 2;
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(color.R, color.G, color.B) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            if (Projectile.localAI[0] >= 20)
                Projectile.Kill();
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Texture2D teleportGlow = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect2 = new(0, 0, teleportGlow.Width, teleportGlow.Height);
            Vector2 origin2 = new(teleportGlow.Width / 2, teleportGlow.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Color colour2 = Color.Lerp(color, color, 1f / GlowTimer * 10f) * (1f / GlowTimer * 20f);
            if (Glow)
            {
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2, Rot, origin2, scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(teleportGlow, position2, new Rectangle?(rect2), colour2 * 0.5f, Rot, origin2, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
    }
}
