using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadestone Crate");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneCrateTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
        }
    }
}