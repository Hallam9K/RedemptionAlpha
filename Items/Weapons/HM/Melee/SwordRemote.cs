using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Buffs.Cooldowns;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class SwordRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cleaver Remote");
            /* Tooltip.SetDefault("'Size does matter'"
                + "\nCalls upon the Omega Cleaver to unleash a devastating attack" +
                "\nRight-Click to switch mode of attack" +
                "\n15 second cooldown"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Default;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 18;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.crit = 4;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.shoot = ModContent.ProjectileType<RemoteCleaver>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool AltFunctionUse(Player player) => true;
        public int AttackMode;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = CustomSounds.ShootChange;
                return true;
            }
            else
                Item.UseSound = SoundID.Item44;
            return !player.HasBuff(ModContent.BuffType<SwordRemoteCooldown>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = 5;
                player.itemTime = 5;
                player.itemAnimation = 5;

                AttackMode++;
                if (AttackMode >= 4)
                    AttackMode = 0;

                switch (AttackMode)
                {
                    case 0:
                        CombatText.NewText(player.getRect(), Color.Red, Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode1"), true, false);
                        break;
                    case 1:
                        CombatText.NewText(player.getRect(), Color.Red, Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode2"), true, false);
                        break;
                    case 2:
                        CombatText.NewText(player.getRect(), Color.Red, Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode3"), true, false);
                        break;
                    case 3:
                        CombatText.NewText(player.getRect(), Color.Red, Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode4"), true, false);
                        break;
                }
            }
            else
            {
                player.AddBuff(ModContent.BuffType<SwordRemoteCooldown>(), 900, true);
                switch (AttackMode)
                {
                    case 0:
                        Projectile.NewProjectile(source, new Vector2(player.Center.X + (50 * player.direction), player.Center.Y - 200), Vector2.Zero, ModContent.ProjectileType<RemoteCleaver>(), 3000, 18, Main.myPlayer, 0);
                        break;
                    case 1:
                        Projectile.NewProjectile(source, new Vector2(player.Center.X + (50 * player.direction), player.Center.Y - 200), Vector2.Zero, ModContent.ProjectileType<RemoteCleaver>(), 3000, 18, Main.myPlayer, 1);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, new Vector2(player.Center.X + (50 * player.direction), player.Center.Y - 200), Vector2.Zero, ModContent.ProjectileType<RemoteCleaver>(), 3000, 18, Main.myPlayer, 2);
                        break;
                    case 3:
                        Projectile.NewProjectile(source, new Vector2(player.Center.X + (50 * player.direction), player.Center.Y - 200), Vector2.Zero, ModContent.ProjectileType<RemoteCleaver>(), 3000, 18, Main.myPlayer, 3);
                        break;
                }
            }
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string shotType = "";
            switch (AttackMode)
            {
                case 0:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode1");
                    break;
                case 1:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode2");
                    break;
                case 2:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode3");
                    break;
                case 3:
                    shotType = Language.GetTextValue("Mods.Redemption.Items.SwordRemote.Mode4");
                    break;
            }
            TooltipLine line = new(Mod, "ShotName", shotType)
            {
                OverrideColor = Color.Red,
            };
            tooltips.Add(line);
        }
    }
    public class RemoteCleaver : ModProjectile
    {
        public float[] oldrot = new float[6];
        public override string Texture => "Redemption/NPCs/Bosses/Cleaver/OmegaCleaver";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Cleaver");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public float rot;
        public int repeat;
        NPC target;
        Vector2 thrustPos1;
        Vector2 thrustPos2;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.frame = 0;
                    switch (Projectile.localAI[0])
                    {
                        case 0:
                            Projectile.localAI[1]++;
                            if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center, true, player.MinionAttackTargetNPC))
                            {
                                rot.SlowRotation(target.RightOf(Projectile) ? 0.78f : 5.49f, (float)Math.PI / 20f);
                                Projectile.rotation = rot;
                                Projectile.Move(target.Center + new Vector2(200 * Projectile.RightOfDir(target), -160), 16, 3);
                            }
                            else
                            {
                                Projectile.velocity *= 0.9f;
                                rot.SlowRotation(player.direction == 1 ? 0.78f : 5.49f, (float)Math.PI / 20f);
                                Projectile.rotation = rot;
                            }
                            Projectile.alpha -= 5;
                            if (Projectile.alpha <= 0)
                            {
                                Projectile.alpha = 0;
                                if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center, true, player.MinionAttackTargetNPC))
                                {
                                    Projectile.rotation = target.RightOf(Projectile) ? 0.78f : 5.49f;
                                    repeat = target.RightOf(Projectile) ? 0 : 1;
                                }
                                else
                                {
                                    Projectile.rotation = player.direction == 1 ? 0.78f : 5.49f;
                                    repeat = player.direction == 1 ? 0 : 1;
                                }
                                Projectile.localAI[0] = 1;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 1:
                            Projectile.localAI[1]++;
                            Projectile.velocity *= 0f;
                            if (Projectile.localAI[1] > 10)
                            {
                                Projectile.friendly = true;
                                SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                                Projectile.localAI[0] = 2;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 2:
                            Projectile.localAI[1]++;
                            Projectile.friendly = true;
                            if (Projectile.localAI[1] == 1) { Projectile.velocity.X = repeat == 0 ? 15 : -15; }
                            if (Projectile.localAI[1] == 11) { Projectile.velocity.X = repeat == 0 ? -15 : 15; }
                            Projectile.velocity.Y = 26;
                            Projectile.rotation += repeat == 0 ? 0.1f : -0.1f;
                            if (Projectile.localAI[1] > 20)
                            {
                                Projectile.friendly = false;
                                Projectile.velocity *= 0f;
                                Projectile.localAI[0] = 3;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 3:
                            Projectile.friendly = false;
                            Projectile.alpha += 5;
                            if (Projectile.alpha >= 255)
                            {
                                Projectile.Kill();
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (Projectile.localAI[0])
                    {
                        case 0:
                            Projectile.frame = 0;
                            Projectile.localAI[1]++;
                            Projectile.alpha -= 5;
                            if (Projectile.alpha <= 0)
                            {
                                Projectile.alpha = 0;
                                Projectile.localAI[0] = 1;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 1:
                            if (++Projectile.frameCounter >= 10)
                            {
                                Projectile.frameCounter = 0;
                                if (++Projectile.frame >= 5)
                                {
                                    Projectile.frame = 1;
                                }
                            }
                            Projectile.localAI[1]++;
                            if (Projectile.localAI[1] < 50)
                            {
                                Projectile.velocity *= .94f;
                                rot.SlowRotation(Projectile.DirectionTo(Main.MouseWorld).ToRotation() + 1.57f, (float)Math.PI / 30f);
                                Projectile.rotation = rot;
                            }
                            else if (Projectile.localAI[1] <= 70)
                            {
                                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
                            }
                            if (Projectile.localAI[1] == 50)
                            {
                                SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 40;
                                Projectile.friendly = true;
                            }
                            if (Projectile.localAI[1] > 70)
                            {
                                Projectile.friendly = false;
                                Projectile.alpha += 5;
                                Projectile.velocity *= .94f;
                                if (Projectile.alpha >= 255)
                                {
                                    Projectile.Kill();
                                }
                            }
                            break;
                    }
                    break;
                case 2:
                    Projectile.frame = 0;
                    switch (Projectile.localAI[0])
                    {
                        case 0:
                            Projectile.localAI[1]++;
                            Projectile.alpha -= 5;
                            if (Projectile.alpha <= 0)
                            {
                                Projectile.alpha = 0;
                                Projectile.localAI[0] = 1;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 1:
                            Projectile.localAI[1]++;
                            rot.SlowRotation(Projectile.DirectionTo(Main.MouseWorld).ToRotation() + 1.57f, (float)Math.PI / 30f);
                            Projectile.rotation = rot;
                            Projectile.velocity *= .96f;
                            if (Projectile.localAI[1] >= 60 && Projectile.localAI[1] % 5 == 0 && Projectile.localAI[1] < 130)
                            {
                                if (Main.myPlayer == Projectile.owner)
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 7), Main.rand.NextFloat(-6, 7)), ModContent.ProjectileType<PhantomCleaver_F>(), 500, Projectile.knockBack, Main.myPlayer);
                            }
                            if (Projectile.localAI[1] > 140)
                            {
                                Projectile.alpha += 5;
                                if (Projectile.alpha >= 255)
                                {
                                    Projectile.Kill();
                                }
                            }
                            break;
                    }
                    break;
                case 3:
                    switch (Projectile.localAI[0])
                    {
                        case 0:
                            Projectile.frame = 0;
                            Projectile.localAI[1]++;
                            Projectile.alpha -= 5;
                            if (Projectile.alpha <= 0)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.NebSound2 with { Volume = .9f }, Projectile.position);
                                Projectile.alpha = 0;
                                Projectile.localAI[0] = 1;
                                Projectile.localAI[1] = 0;
                            }
                            break;
                        case 1:
                            if (++Projectile.frameCounter >= 10)
                            {
                                Projectile.frameCounter = 0;
                                if (++Projectile.frame >= 5)
                                {
                                    Projectile.frame = 1;
                                }
                            }
                            Projectile.localAI[1]++;
                            if (Projectile.localAI[1] == 40)
                            {
                                if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center, true, player.MinionAttackTargetNPC))
                                {
                                    if (Projectile.Center.X < target.Center.X)
                                        repeat = 1;
                                }
                                if (Main.myPlayer == Projectile.owner)
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y) + RedeHelper.PolarVector(134, Projectile.rotation + (float)-Math.PI / 2), RedeHelper.PolarVector(9, Projectile.rotation + (float)-Math.PI / 2), ModContent.ProjectileType<RedPrism_F>(), 1400, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
                            }
                            if (Projectile.localAI[1] > 40)
                            {
                                if (repeat == 1) { Projectile.rotation += 0.01f; }
                                else { Projectile.rotation -= 0.01f; }
                                Projectile.rotation *= 1.04f;
                            }
                            if (Projectile.localAI[1] > 120)
                            {
                                Projectile.localAI[0] = 2;
                            }
                            break;
                        case 2:
                            if (Projectile.localAI[1] > 90)
                            {
                                Projectile.alpha += 5;
                                if (Projectile.alpha >= 255)
                                {
                                    Projectile.Kill();
                                }
                            }
                            break;
                    }
                    break;
            }
            bigFlareOpacity += Main.rand.NextFloat(-.1f, .1f);
            bigFlareOpacity = MathHelper.Clamp(bigFlareOpacity, .9f, 1.1f);
            thrustPos1 = Projectile.Center + RedeHelper.PolarVector(26, Projectile.rotation) + RedeHelper.PolarVector(-74, Projectile.rotation - (float)Math.PI / 2) + Projectile.velocity;
            thrustPos2 = Projectile.Center + RedeHelper.PolarVector(-26, Projectile.rotation) + RedeHelper.PolarVector(-74, Projectile.rotation - (float)Math.PI / 2) + Projectile.velocity;
            for (int i = 0; i < 4; i++)
                RedeParticleManager.CreateGlowParticle(thrustPos1, RedeHelper.PolarVector(Main.rand.Next(6, 15), Projectile.rotation + (float)Math.PI / 2 + Main.rand.NextFloat(-.01f, .01f)) - (Projectile.velocity / 4), 1f * Projectile.Opacity, RedeParticleManager.redThrusterColors, Main.rand.Next(10, 30), layer: Layer.BeforeNPCs);
            for (int i = 0; i < 4; i++)
                RedeParticleManager.CreateGlowParticle(thrustPos2, RedeHelper.PolarVector(Main.rand.Next(6, 15), Projectile.rotation + (float)Math.PI / 2 + Main.rand.NextFloat(-.01f, .01f)) - (Projectile.velocity / 4), 1f * Projectile.Opacity, RedeParticleManager.redThrusterColors, Main.rand.Next(10, 30), layer: Layer.BeforeNPCs);

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.5f, 0).RotatedBy(Projectile.rotation + -MathHelper.PiOver2);
            float point = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - unit * 72,
                Projectile.Center + unit * 72, 66, ref point))
                return true;
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor * Projectile.Opacity;
        }
        public float bigFlareOpacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Cleaver/OmegaCleaver_Glow").Value;
            Texture2D trail = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Cleaver/OmegaCleaver_Trail").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                Main.EntitySpriteDraw(trail, value4 + Projectile.Size / 2f - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.VlitchGlowColour) * 0.5f, oldrot[i], texture.Size() / 2, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowMask, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(RedeColor.RedPulse), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);

            Texture2D bigFlareAni = ModContent.Request<Texture2D>("Redemption/Textures/BigFlare").Value;
            Vector2 origin2 = new(bigFlareAni.Width / 2f, bigFlareAni.Height / 2f);
            Vector2 flarePos1 = thrustPos1 + RedeHelper.PolarVector(-6, Projectile.rotation - (float)Math.PI / 2);
            Vector2 flarePos2 = thrustPos2 + RedeHelper.PolarVector(-6, Projectile.rotation - (float)Math.PI / 2);
            Main.EntitySpriteDraw(bigFlareAni, flarePos1 - Main.screenPosition, null, Projectile.GetAlpha(Color.IndianRed with { A = 0 }) * bigFlareOpacity, 0, origin2, Projectile.scale * bigFlareOpacity * .4f, effects, 0);
            Main.EntitySpriteDraw(bigFlareAni, flarePos2 - Main.screenPosition, null, Projectile.GetAlpha(Color.IndianRed with { A = 0 }) * bigFlareOpacity, 0, origin2, Projectile.scale * bigFlareOpacity * .4f, effects, 0);
            Main.EntitySpriteDraw(bigFlareAni, flarePos1 - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }) * bigFlareOpacity, 0, origin2, Projectile.scale * bigFlareOpacity * .2f, effects, 0);
            Main.EntitySpriteDraw(bigFlareAni, flarePos2 - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }) * bigFlareOpacity, 0, origin2, Projectile.scale * bigFlareOpacity * .2f, effects, 0);
            return false;
        }
    }
}
