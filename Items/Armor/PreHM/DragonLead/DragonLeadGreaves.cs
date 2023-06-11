using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using System.Collections.Generic;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Armor.PreHM.DragonLead
{
    [AutoloadEquip(EquipType.Legs)]
    public class DragonLeadGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon-Lead Greaves");
            /* Tooltip.SetDefault("8% increased critical strike chance\n" +
                "10% increased movement speed"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 8;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 15)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.DragonLead.DragonLeadGreaves"))
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
