using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Armor.PreHM.DragonLead
{
    [AutoloadEquip(EquipType.Head)]
    public class DragonLeadSkull : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon-Lead Skull");
            // Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DragonLeadRibplate>() && legs.type == ModContent.ItemType<DragonLeadGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.DragonLead.20Increased") + ElementID.IceS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance") +
                Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.DragonLead.Bonus");
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Ice] += 0.2f;
            player.RedemptionPlayerBuff().dragonLeadBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.SpecialTooltips.DragonLead.DragonLeadSkull"))
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
