using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class KeepersKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Keeper's Knife");
            /* Tooltip.SetDefault("Hitting enemies inflict Necrotic Gouge\n" +
                "Deals double damage to undead and skeletons" +
                "\n'O murderer, let my knife pierce true'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 46;
            Item.height = 36;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 8);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 23;
            Item.knockBack = 4;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<KeepersKnife_Slash>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity += velocity.RotatedByRandom(0.2f);
            Vector2 Offset = Vector2.Normalize(velocity) * 50f;
            position += Offset;
        }
        /*
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GrimShard>())
                .AddIngredient(ItemID.DemoniteBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GrimShard>())
                .AddIngredient(ItemID.CrimtaneBar, 8)
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
