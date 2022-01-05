using Redemption.Tiles.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Uranium : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Uranium");
            Tooltip.SetDefault("Holding this may cause radiation poisoning without proper equipment");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Lime;
            Item.value = 2000;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<UraniumTile>();
            Item.ammo = Item.type;
        }
    }
}

