using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForgottenSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sword of the Forgotten");
            // Tooltip.SetDefault("Spins the blade around the player and sets ablaze, flinging embers in its wake");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 68;
            Item.height = 68;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 65);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.UseSound = SoundID.DD2_BetsyWindAttack;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 28;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ForgottenSword_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Found in the Demonhollow by a set of human explorers from Anglon. The weapon is crafted from bone and\n" +
                    "a metal named \"Charred Iron\" by those whom discovered it, and was attributed to a fabled blacksmith named\n" +
                    "Ophos due to its use of Firestorm magic. Yet the lack of many of his trademarks put doubt on that claim.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
            TooltipLine slashLine = new(Mod, "SharpBonus", Language.GetTextValue("Mods.Redemption.GenericTooltips.Bonuses.SlashBonus")) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
