using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ElectricHazard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Hazard");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 3));

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 0, 15, 0);
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.createTile = ModContent.TileType<ElectricHazardTile>();
        }
    }
}
