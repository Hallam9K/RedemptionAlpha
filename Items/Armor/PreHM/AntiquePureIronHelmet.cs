using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using System;

namespace Redemption.Items.Armor.PreHM
{
	[AutoloadEquip(EquipType.Head)]
	public class AntiquePureIronHelmet : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Antique Pure-Iron Helmet");
			Tooltip.SetDefault("");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 26;
			Item.sellPrice(silver: 90);
			Item.rare = ItemRarityID.Orange;
			//Item.defense = 5;
		}

        /*public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "''")
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
        }*/
    }
}