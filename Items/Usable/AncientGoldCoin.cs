using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Usable
{
    public class AncientGoldCoin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antique Dorul");
            Tooltip.SetDefault("'Ancient gold coins used in the olden days of Gathuram'" +
                "\nCan be given to a certain Undead as currency");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6));

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;
            Item.maxStack = 999;
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
