using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Items.Weapons.PreHM.Melee;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class Midnight : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Midnight, Defiler of the Prince");
            /* Tooltip.SetDefault("Slams down in a blaze of twilight" +
                "\nHold left-click to charge a stronger slam, releasing Nebula stars if hitting a surface or enemy" +
                "\nNebula Stars slowly chase targets while spewing nebula sparks" +
                "\nCan't create nebula stars while at least 4 are already active"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 58;
            Item.height = 52;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 105;
            Item.knockBack = 10.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Midnight_SlashProj>();
            Item.Redemption().TechnicallyAxe = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<NoblesHalberd>())
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.Midnight.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}
