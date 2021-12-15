using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Materials.HM;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Shield)]
    public class InfectionShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infected Thornshield");
            Tooltip.SetDefault("Double tap a direction to dash"
                + "\n8% increased melee critical strike chance"
                + "\nInflicts Infection upon dashing into an enemy"
                + "\nReleases acid-like sparks as you move");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.rare = ItemRarityID.Lime;
            Item.value = 80000;
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee;
            Item.accessory = true;
            Item.crit = 4;
            Item.knockBack = 10;
            Item.expert = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EoCShield)
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 10)
                .AddIngredient(ModContent.ItemType<StarliteBar>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.rand.NextBool(10))
                {
                    Projectile.NewProjectile(player.GetProjectileSource_Accessory(Item), new Vector2(player.position.X + Main.rand.NextFloat(player.width), player.position.Y + Main.rand.NextFloat(player.height)), new Vector2(0f, 0f), ModContent.ProjectileType<InfectionShield_AcidSpark>(), 0, 0, Main.myPlayer);
                }
            }
            player.GetModPlayer<ThornshieldDashPlayer>().DashAccessoryEquipped = true;
            player.GetCritChance(DamageClass.Melee) += 8;
        }
    }
    public class ThornshieldDashPlayer : ModPlayer
    {
        public const int DashRight = 0;
        public const int DashLeft = 1;

        public const int DashCooldown = 50;
        public const int DashDuration = 35;

        public const float DashVelocity = 11f;

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
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 0, default, 1f);
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 0, default, 1f);
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].position.X += Main.rand.Next(-5, 6);
                    Main.dust[dust].position.Y += Main.rand.Next(-5, 6);
                    Main.dust[dust].velocity *= 0.2f;
                }
            }

            if (DashDelay > 0)
                DashDelay--;

            if (DashTimer > 0)
            {
                Player.eocDash = DashTimer;
                Player.armorEffectDrawShadowEOCShield = true;

                if (ShieldHit < 0)
                {
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                    Dust.NewDust(Player.position, Player.width, Player.height, DustID.GreenFairy);
                    Rectangle hitbox = new((int)(Player.position.X + Player.velocity.X * 0.5 - 4), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4), Player.width + 8, Player.height + 8);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.dontTakeDamage || npc.friendly)
                            continue;

                        if (!hitbox.Intersects(npc.Hitbox) || !npc.noTileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height))
                            continue;

                        float damage = 40 * Player.GetDamage(DamageClass.Melee);
                        float knockback = 10;
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
                            npc.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 600);
                            if (Main.rand.NextBool(5))
                                npc.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 300);

                            npc.StrikeNPC((int)damage, knockback, hitDirection, crit);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, knockback, hitDirection, 0,
                                    0, 0);
                        }

                        Player.dashDelay = 30;
                        Player.velocity.X = -hitDirection * 1f;
                        Player.velocity.Y = -4f;
                        Player.immune = true;
                        Player.immuneTime = 10;
                        ShieldHit = i;
                    }
                }

                DashTimer--;
            }
        }

        private bool CanUseDash()
        {
            return DashAccessoryEquipped
                && Player.dashType == 0
                && !Player.setSolar
                && !Player.mount.Active;
        }
    }
    public class InfectionShield_AcidSpark : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Spark");
        }
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            int DustID2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 20, default, 0.7f);
            Main.dust[DustID2].noGravity = true;
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                NPC npc = Main.npc[p];
                if (!npc.active || npc.immortal || npc.dontTakeDamage || npc.friendly || !npc.CanBeChasedBy() || !Projectile.Hitbox.Intersects(npc.Hitbox))
                    continue;

                npc.AddBuff(ModContent.BuffType<BileDebuff>(), 300);
                SoundEngine.PlaySound(npc.HitSound.WithVolume(0.4f), npc.position);
                Projectile.Kill();
            }
        }
    }
}
