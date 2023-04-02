using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs;
using Redemption.Buffs.Debuffs;
using Redemption.NPCs.Bosses.Cleaver;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class ObliterationDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Double tap a direction while airborne to dash\n" +
                "Dashing through enemies gives the player a stack of Obliteration Motivation" +
                "\nObliteration Motivation increases damage, defense and reduces the dash cooldown, at the cost of decreased life regen" +
                "\nStacks up to 5 times"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ObliterationDashPlayer>().DashAccessoryEquipped = true;
        }
    }
    public class ObliterationDashPlayer : ModPlayer
    {
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 180;
        public const int DashDuration = 30;

        public const float DashVelocity = 18f;

        public int DashDir = -1;

        public bool DashAccessoryEquipped;
        public bool HitOnce;
        public int DashDelay = 0;
        public int DashTimer = 0;

        public int MotivationStack;

        public override void ResetEffects()
        {
            DashAccessoryEquipped = false;

            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
                DashDir = DashRight;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
                DashDir = DashLeft;
            else
                DashDir = -1;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (DashTimer > 5)
                Player.gravity = 0;
        }
        private readonly List<int> npcsHit = new();
        public override void PreUpdateMovement()
        {
            Point tileBelow = Player.Bottom.ToTileCoordinates();
            if (CanUseDash() && DashDir != -1 && DashDelay == 0 && !Main.tile[tileBelow.X, tileBelow.Y].HasUnactuatedTile)
            {
                Vector2 newVelocity = Player.velocity;
                npcsHit.Clear();
                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * (DashVelocity * 1.2f);
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown - (12 * MotivationStack);
                DashTimer = DashDuration;
                Player.velocity = newVelocity;
                Player.velocity.Y /= 10;
                HitOnce = false;
                SoundEngine.PlaySound(SoundID.Item74, Player.position);
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain, Scale: 2);
                    Main.dust[dust].velocity *= 0.2f;
                    Main.dust[dust].noGravity = true;
                }
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashDelay == 1)
                SoundEngine.PlaySound(CustomSounds.OODashReady with { Volume = 0.6f, Pitch = 0.3f }, Player.position);

            if (DashTimer > 0)
            {
                Player.eocDash = DashTimer - 1;
                Player.armorEffectDrawShadowEOCShield = true;
                if (DashTimer > 5)
                {
                    if (Player.velocity.X >= -3 && Player.velocity.X <= 3)
                        DashTimer = 0;

                    if (DashTimer <= 27 && DashTimer % 3 == 0 && (Player.velocity.X < -8 || Player.velocity.X > 8))
                    {
                        SoundEngine.PlaySound(SoundID.Item91, Player.position);
                        int p = Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<ObliterationDrive>())), Player.Center, new Vector2(-3f * Player.direction, 10f), ModContent.ProjectileType<OmegaBlast>(), 200, 4, Main.myPlayer);
                        Main.projectile[p].hostile = false;
                        Main.projectile[p].friendly = true;
                        Main.projectile[p].DamageType = DamageClass.Melee;
                        p = Projectile.NewProjectile(Player.GetSource_Accessory(new Item(ModContent.ItemType<ObliterationDrive>())), Player.Center, new Vector2(-3f * Player.direction, -10f), ModContent.ProjectileType<OmegaBlast>(), 200, 4, Main.myPlayer);
                        Main.projectile[p].hostile = false;
                        Main.projectile[p].friendly = true;
                        Main.projectile[p].DamageType = DamageClass.Melee;
                    }
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain);
                    Main.dust[d].noGravity = true;
                    Rectangle hitbox = Player.Hitbox;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.dontTakeDamage || npc.friendly)
                            continue;

                        if (!hitbox.Intersects(npc.Hitbox) || !npc.noTileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height))
                            continue;

                        if (npc.CountsAsACritter && Player.dontHurtCritters)
                            continue;

                        float damage = 260 * Player.GetDamage(DamageClass.Melee).Additive;
                        float knockback = 13 * Player.GetKnockback(DamageClass.Melee).Additive;
                        bool crit = false;

                        if (Main.rand.Next(100) < Player.GetCritChance(DamageClass.Melee))
                            crit = true;

                        int hitDirection = Player.velocity.X < 0f ? -1 : 1;

                        if (Player.whoAmI == Main.myPlayer && !npcsHit.Contains(i))
                        {
                            if (!HitOnce)
                            {
                                if (!Player.HasBuff(ModContent.BuffType<OblitBuff1>()) && !Player.HasBuff(ModContent.BuffType<OblitBuff2>()) && !Player.HasBuff(ModContent.BuffType<OblitBuff3>()) && !Player.HasBuff(ModContent.BuffType<OblitBuff4>()) && !Player.HasBuff(ModContent.BuffType<OblitBuff5>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff1>(), 600);
                                else if (Player.HasBuff(ModContent.BuffType<OblitBuff1>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff2>(), 600);
                                else if (Player.HasBuff(ModContent.BuffType<OblitBuff2>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff3>(), 600);
                                else if (Player.HasBuff(ModContent.BuffType<OblitBuff3>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff4>(), 600);
                                else if (Player.HasBuff(ModContent.BuffType<OblitBuff4>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff5>(), 600);
                                else if (Player.HasBuff(ModContent.BuffType<OblitBuff5>()))
                                    Player.AddBuff(ModContent.BuffType<OblitBuff5>(), 600);
                                HitOnce = true;
                            }
                            npcsHit.Add(npc.whoAmI);
                            BaseAI.DamageNPC(npc, (int)damage, knockback, hitDirection, Player, crit: crit);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, knockback, hitDirection, 0,
                                    0, 0);
                        }

                        Player.immune = true;
                        Player.immuneTime = 20;
                        Player.dashDelay = 30;
                    }
                }
                DashTimer--;
            }
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (damageSource.SourceNPCIndex >= 0 && DashTimer >= 5)
                return true;
            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && !Player.mount.Active
                && !Player.HasBuff(ModContent.BuffType<StunnedDebuff>());
        }
    }
}