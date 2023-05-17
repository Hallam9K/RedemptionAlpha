using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class ObsidianTecpatl : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 28;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 15);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 28;
            Item.crit = 12;
            Item.knockBack = 3;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ObsidianTecpatl_Slash>();
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
                .AddIngredient(ItemID.Obsidian, 20)
                .AddTile(TileID.Furnaces)
                .Register();
        }*/
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
