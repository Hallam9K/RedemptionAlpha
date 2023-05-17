using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BlazingBalisong : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Plants hellfire charges onto the enemy\n" +
                "Upon the player's Spirit Level increasing, all hellfire charges explode"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 30;
            Item.height = 32;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 37;
            Item.knockBack = 3;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BlazingBalisong_Slash>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity += velocity.RotatedByRandom(0.6f);
            Vector2 Offset = Vector2.Normalize(velocity) * 50f;
            position += Offset;
        }
        /*
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ModContent.ItemType<ObsidianTecpatl>())
                .AddTile(TileID.Anvils)
                .Register();
        }*/
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
