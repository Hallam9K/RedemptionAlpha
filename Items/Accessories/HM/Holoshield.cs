using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Accessories.PostML;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Shield)]
    public class Holoshield : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("6% damage reduction"
                + "\nDouble tap a direction to dash" +
                "\nDashing into projectiles will reflect them" +
                "\nCan't reflect projectiles exceeding 200 damage"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 22;
            Item.height = 26;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HoloshieldDashPlayer>().DashAccessoryEquipped = true;
            player.endurance += 0.06f;
        }
    }
    public class HoloshieldDashPlayer : ModPlayer
    {
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 50;
        public const int DashDuration = 35;

        public const float DashVelocity = 10f;

        public int DashDir = -1;

        public bool DashAccessoryEquipped;
        public int DashDelay = 0;
        public int DashTimer = 0;
        public int ShieldHit;

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

        public override void PreUpdateMovement()
        {
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return;
                }

                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                ShieldHit = -1;
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            {
                Player.eocDash = DashTimer - 1;
                Player.armorEffectDrawShadowEOCShield = true;

                if (ShieldHit < 0 && DashTimer > 15)
                {
                    Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5 - 4), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.dontTakeDamage || npc.friendly || NPCLoader.CanBeHitByItem(npc, Player, new Item(ModContent.ItemType<Holoshield>())) == false)
                            continue;

                        if (!hitbox.Intersects(npc.Hitbox) || !npc.noTileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height))
                            continue;

                        if ((npc.CountsAsACritter || npc.lifeMax <= 5) && Player.dontHurtCritters)
                            continue;

                        float damage = 20 * Player.GetDamage(DamageClass.Melee).Additive;
                        float knockback = 8;
                        bool crit = false;

                        if (Player.kbGlove)
                            knockback *= 2f;
                        if (Player.kbBuff)
                            knockback *= 1.5f;

                        if (Main.rand.Next(100) < Player.GetCritChance(DamageClass.Melee))
                            crit = true;

                        int hitDirection = Player.velocity.X < 0f ? -1 : 1;

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            BaseAI.DamageNPC(npc, (int)damage, knockback, hitDirection, Player, crit: crit);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, knockback, hitDirection, 0,
                                    0, 0);
                        }

                        Player.immune = true;
                        Player.immuneTime = 20;
                        Player.dashDelay = 30;
                        Player.velocity.X = -Player.velocity.X;
                        Player.velocity.Y = -4f;
                        ShieldHit = 1;
                        DashTimer = 0;
                    }
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.active || !proj.hostile || proj.friendly || proj.damage >= 100 || proj.velocity.Length() <= 0)
                            continue;

                        if (!hitbox.Intersects(proj.Hitbox) || proj.tileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, proj.position, proj.width, proj.height))
                            continue;

                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.Reflect);
                        proj.damage *= 8;
                        proj.velocity = -proj.velocity;
                        proj.friendly = true;
                        proj.hostile = false;

                        Player.immune = true;
                        Player.immuneTime = 20;
                        Player.dashDelay = 30;
                        Player.velocity.X = -Player.velocity.X;
                        Player.velocity.Y = -4f;
                        ShieldHit = 1;
                        DashTimer = 0;
                    }
                }
                DashTimer--;
            }
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if ((damageSource.SourceNPCIndex >= 0 || (damageSource.SourceProjectileLocalIndex >= 0 && Main.projectile[damageSource.SourceProjectileLocalIndex].damage < 200)) && ShieldHit < 0 && DashTimer > 15)
                return true;
            return false;
        }
        private bool CanUseDash()
        {
            Point tileBelow = Player.Bottom.ToTileCoordinates();
            if (Player.GetModPlayer<ObliterationDashPlayer>().DashAccessoryEquipped && Player.GetModPlayer<ObliterationDashPlayer>().DashDelay == 0 && !Main.tile[tileBelow.X, tileBelow.Y].HasUnactuatedTile)
                return false;
            if (Player.dashType == 1 || Player.dashType == 3)
                return false;
            return DashAccessoryEquipped
                && !Player.mount.Active
                && !Player.HasBuff(ModContent.BuffType<StunnedDebuff>())
                && !Player.GetModPlayer<ThornshieldDashPlayer>().DashAccessoryEquipped;
        }
    }
}
