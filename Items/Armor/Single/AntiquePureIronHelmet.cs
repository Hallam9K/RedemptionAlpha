using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Redemption.Items.Armor.PreHM.PureIron;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
	public class AntiquePureIronHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Antique Pure-Iron Helmet");
            // Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PureIronHelmet>();
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 26;
			Item.sellPrice(silver: 90);
			Item.rare = ItemRarityID.Orange;
			Item.defense = 5;
		}

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PureIronChestplate>() && legs.type == ModContent.ItemType<PureIronGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.PureIron.20Increased") + ElementID.FireS + Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.VanillaArmor.Resistance") +
                Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.PureIron.Bonus");
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Fire] += 0.2f;
            player.RedemptionPlayerBuff().pureIronBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    Language.GetTextValue("Mods.Redemption.SpecialTooltips.PureIron.AntiquePureIronHelmet"))
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
