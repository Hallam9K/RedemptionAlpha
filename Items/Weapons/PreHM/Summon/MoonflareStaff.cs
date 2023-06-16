using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class MoonflareStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moonflare Staff");
            /* Tooltip.SetDefault("Summons a Moonflare Guardian to fight for you\n" +
                "The guardian will buff other minions while the moon is out, giving +3 damage and making them emit light"); */
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Summon;
            Item.width = 38;
            Item.height = 36;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = 800;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<MoonflareGuardianBuff>();
            Item.shoot = ModContent.ProjectileType<MoonflareGuardian>();
            Item.mana = 10;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = "There is no moonlight to reflect...";
            if (Main.dayTime || Main.moonPhase == 4)
            {
                TooltipLine line = new(Mod, "text", text)
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Insert(2, line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MoonflareFragment>(), 10)
                .AddTile(TileID.Anvils)
                .AddCondition(RedeConditions.InMoonlight)
                .Register();
        }
    }
}