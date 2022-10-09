using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class HalogenLamp : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halogen Lamp"); 
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HalogenLampTile>(), 0);
            Item.width = 12;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(0, 0, 2, 0);
        }
    }
}
