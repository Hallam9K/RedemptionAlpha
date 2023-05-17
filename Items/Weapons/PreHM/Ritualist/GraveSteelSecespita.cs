using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class GraveSteelSecespita : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Grave Steel Secespita");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 26;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 11;
            Item.knockBack = 3;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<GraveSteelSecespita_Slash>();
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
                .AddIngredient(ModContent.ItemType<GraveSteelAlloy>(), 12)
                .AddIngredient(ModContent.ItemType<LostSoul>())
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
