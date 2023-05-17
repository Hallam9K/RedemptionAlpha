using Microsoft.Xna.Framework;
using Redemption.DamageClasses;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BuddingBoline : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("After the player's Spirit Level increases, the following successful hit spawns a flower\n" +
                "The flower gives slightly increased life regeneration and knockback immunity while in its radius"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 28;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 95);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 30;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BuddingBoline_Slash>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 50f;
            position += Offset;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
