using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class SwordSlicer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Zephos' Sword Slicer");
            /* Tooltip.SetDefault("Hitting armed enemies inflicts Disarmed\n" +
                "Disarmed heavily decreases contact damage and their weapon damage\n" +
                "Blocks weak physical projectiles"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SilverwoodBow>();
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 46;
            Item.height = 46;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 44;
            Item.knockBack = 4;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<SwordSlicer_Slash>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Given to Zephos by Draven, his uncle, during training. The slit in the middle is used to catch the opponent's blades,\n" +
                    "one with great strength can use this advantage to twist the opponent's blade until it snaps, leaving them disarmed.'")
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
            TooltipLine slashLine = new(Mod, "SharpBonus", RedeItem.slashBonus) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
