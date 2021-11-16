using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.DamageClasses;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Misc;
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
                + "\n14% increased druidic critical strike chance"
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
            Item.DamageType = ModContent.GetInstance<DruidClass>();
            Item.accessory = true;
            Item.crit = 4;
            Item.knockBack = 10;
            Item.expert = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EoCShield)
                .AddIngredient(ModContent.ItemType<Xenomite>(), 10)
                .AddIngredient(ModContent.ItemType<Starlite>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DashPlayer modPlayer = player.GetModPlayer<DashPlayer>();
            if (Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                if (Main.rand.Next(10) == 0)
                {
                    Projectile.NewProjectile(player.GetProjectileSource_Accessory(Item), new Vector2(player.position.X + Main.rand.NextFloat(player.width), player.position.Y + Main.rand.NextFloat(player.height)), new Vector2(0f, 0f), ModContent.ProjectileType<InfectionShield_AcidSpark>(), 0, 0, Main.myPlayer);
                }
            }
            modPlayer.infectedThornshield = true;
            player.GetCritChance<DruidClass>() += 14;
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
