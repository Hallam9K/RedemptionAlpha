using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Tiles.Tiles;

namespace Redemption.Items.Usable
{
    public class AncientGoldCoin : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Antique Dorul");
            /* Tooltip.SetDefault("Can be given to a certain Undead as currency\n" +
                "'Ancient gold coins used in the olden days of Gathuram'"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6));

            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Gray;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.createTile = ModContent.TileType<AncientGoldCoinPileTile>();
        }
    }
}
