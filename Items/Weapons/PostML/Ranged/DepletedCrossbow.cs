using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class DepletedCrossbow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Depleted Crossbow");
            /* Tooltip.SetDefault("Fires depleted uranium rods that explode upon impact\n" +
                "Consumes uranium as ammo\n" +
                "No ammo cost if the user has at least 10 uranium"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 68;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 7, 50);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item89;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 300;
            Item.crit = 46;
            Item.knockBack = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<Uranium_Proj>();
            Item.useAmmo = ModContent.ItemType<Uranium>();
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            int uranium = player.FindItem(ModContent.ItemType<Uranium>());
            if (uranium >= 0 && player.inventory[uranium].stack >= 10)
                return false;
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.DepletedCrossbow.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.DepletedCrossbow.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}
