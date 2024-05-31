using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Particles;
using Redemption.Projectiles.Melee;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class SlayerFist_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjExplosive[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
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
        float damageBoost;
        float OpacityTimer;
        bool fullCharge;
        bool launch;
        bool rotRight;
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
                    Projectile.Center = armCenter + dirVec * 15;

                    Vector2 rand = Projectile.position + new Vector2(Main.rand.Next(-1, 1), Main.rand.Next(-1, 1));
                    Projectile.position = rand;
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
            Player.ChangeDir(Projectile.spriteDirection);

            progress = MathHelper.Clamp(timer / (SetSwingSpeed(1) * 90), 0, 1);
            if (Player.channel)
                timer++;
            else
            {
                launchVec = Player.Center.DirectionTo(Main.MouseWorld);
                dirVec = launchVec;
                damageBoost = progress + 1;
                launch = true;
                timer = 0;
            }

            if (!fullCharge && progress >= 1)
            {
                fullCharge = true;
                Projectile.damage *= 4;
                SoundEngine.PlaySound(CustomSounds.ShootChange, Projectile.position);
            }
        }
        public void Launch()
        {
            OpacityTimer++;
            if (!fullCharge)
            {
                if (timer++ == 0)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.MissileFire1, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(15, (Main.MouseWorld - Player.Center).ToRotation()), ModContent.ProjectileType<KS3_FistF>(), (int)(Projectile.damage * damageBoost), Projectile.knockBack, Player.whoAmI);
                }
                else if (timer > 25)
                    Projectile.Kill();
            }
            else
            {
                if(timer++ == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                }
                if (Projectile.alpha >= 255)
                {
                    if (timer >= 16 && timer <= 35)
                    {
                        Player.velocity *= 0.9f;
                        Player.fullRotation += rotRight ? 0.33f : -0.33f;
                        Player.fullRotationOrigin = new Vector2(10, 20);
                    }
                    if (timer >= 35)
                        Projectile.Kill();
                }
                else
                {
                    if (timer <= 20)
                    {
                        MakeDust();
                        Projectile.friendly = true;
                        Player.velocity = launchVec * 30;
                        Player.GetModPlayer<RedePlayer>().fallSpeedIncrease += 30;
                    }
                    else if (timer <= 35)
                    {
                        Projectile.friendly = false;
                        Player.velocity *= 0.9f;
                    }
                    if (timer >= 30)
                        Projectile.Kill();
                }
            }
        }
        public void MakeDust()
        {
            Player.Redemption().contactImmune = true;
            Player.GetModPlayer<RedePlayer>().fallSpeedIncrease += 30;
            Vector2 position = Player.Center + (Vector2.Normalize(Player.velocity) * 30f);
            Dust dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.Frost)];
            dust.position = position;
            dust.velocity = (Player.velocity.RotatedBy(1.57) * 0.33f) + (Player.velocity / 4f);
            dust.position += Player.velocity.RotatedBy(1.57);
            dust.fadeIn = 0.5f;
            dust.noGravity = true;
            dust = Main.dust[Dust.NewDust(Player.position, Player.width, Player.height, DustID.Frost)];
            dust.position = position;
            dust.velocity = (Player.velocity.RotatedBy(-1.57) * 0.33f) + (Player.velocity / 4f);
            dust.position += Player.velocity.RotatedBy(-1.57);
            dust.fadeIn = 0.5f;
            dust.noGravity = true;

            if (timer % 4 == 3)
                ParticleSystem.NewParticle(Main.rand.NextVector2FromRectangle(Player.Hitbox), -Player.velocity * 3, new SpeedParticle(), Color.Cyan, 1);
        }
        public override void OnKill(int timeLeft)
        {
            Player.fullRotation = 0f;
        }
        public override bool? CanHitNPC(NPC target) => launch? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].velocity *= 4;
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].velocity *= 8;
                Main.dust[dust].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
            target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 10);

            Projectile.friendly = false;
            if (fullCharge)
            {
                if (target.Center.X < Player.Center.X)
                    rotRight = true;

                Projectile.alpha = 255;
                Player.velocity = -launchVec * 30;
                Player.velocity.Y -= 16;
                timer = 16;
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
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Texture2D flare2 = ModContent.Request<Texture2D>("Redemption/Textures/WhiteEyeFlare", AssetRequestMode.ImmediateLoad).Value;
            Texture2D trail = ModContent.Request<Texture2D>("Redemption/Textures/Ray", AssetRequestMode.ImmediateLoad).Value;

            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Rectangle rect2 = new(0, 0, 122, 122);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 origin2 = new(Projectile.width / 4, 0);

            Color colour = Color.Lerp(Color.White, new Color(140, 255, 242), 1f / OpacityTimer * 10f) * (1f / OpacityTimer * 10f);

            float opacity = MathHelper.Lerp(2, 0, OpacityTimer / 24);
            float scale = MathHelper.Lerp(1, 0, OpacityTimer / 24);

            float opacity2 = MathHelper.Lerp(2, 0, OpacityTimer / 24);
            float scale2 = MathHelper.Lerp(1, 0, OpacityTimer / 24);

            Vector2 position = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            if (!fullCharge && launch)
            {
                Main.EntitySpriteDraw(flare2, position, new Rectangle?(rect2), colour * opacity2, Projectile.rotation, new Vector2(61, 61), 0.3f, 0, 0);
                Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.5f * opacity2, Projectile.rotation, origin, 0.15f * scale2, 0, 0);
            }
            if (fullCharge && launch)
            {
                Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.5f * opacity, Projectile.rotation, origin, 0.4f * scale, 0, 0);
                Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.25f * opacity, Projectile.rotation, origin, 0.5f * scale, 0, 0);
                Main.EntitySpriteDraw(trail, position, new Rectangle?(rect), colour * 0.5f * opacity, Projectile.rotation, origin2, 1.2f * scale, 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
    }
}