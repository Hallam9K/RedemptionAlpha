using Microsoft.Xna.Framework;
using Redemption.Tiles.Furniture.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class SunkenCaptainPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sunken Captain");
            // Tooltip.SetDefault("'M. Tea'");
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SunkenCaptainPaintingTile>(), 0);
			Item.width = 38;
			Item.height = 38;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.White;
			Item.value = Item.buyPrice(0, 0, 50, 0);
		}
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Once a great captain, a legend to some," +
                    "\nEven after death would he still sail the seas,\n" +
                    "Leading his phantom crew to unknown lands.\n" +
                    "Only under the brightest light of the moon\n" +
                    "Could they set a foot on a seashore.\n" +
                    "Until then, they were ever chasing waves\n" +
                    "With the thickest of fog keeping them company.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "On the backside there's a note, hold [Shift] to read")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }
    }
}