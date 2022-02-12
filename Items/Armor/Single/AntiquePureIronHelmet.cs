using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Armor.PreHM.PureIron;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Items.Armor.Single
{
    [AutoloadEquip(EquipType.Head)]
	public class AntiquePureIronHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Antique Pure-Iron Helmet");
            Tooltip.SetDefault("7% increased damage");
            ArmorIDs.Head.Sets.DrawHead[Mod.GetEquipSlot(Name, EquipType.Head)] = false;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
            player.setBonus = "20% increased Fire elemental resistance\n" +
                "33% chance for weapons to inflict Pure Chill\n" +
                "100% chance for all Pure-Iron weapons to inflict Pure Chill";
            player.RedemptionPlayerBuff().ElementalResistance[1] += 0.2f;
            player.RedemptionPlayerBuff().pureIronBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'An outdated design of the Iron Realm's warrior's helmet.\n" +
                    "Discovered in the Catacombs of Gathuram by Happins, a fallen.\n" +
                    "This design has fur to keep the neck and shoulders warm in the harsh environment'")
                {
                    overrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}