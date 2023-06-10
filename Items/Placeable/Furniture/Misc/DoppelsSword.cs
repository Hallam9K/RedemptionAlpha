using Microsoft.Xna.Framework;
using Redemption.Tiles.Furniture.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class DoppelsSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Steel Sword Fragment");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<DoppelsSwordTile>(), 0);
            Item.width = 46;
            Item.height = 50;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A strange sword from the Silver Age, made of shining steel. Marks of battle are scarce,\n" +
                    "as it was seldom used by its wielder. It feels oddly nostalgic.'")
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
        }
    }
}
