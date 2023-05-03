using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickOrnate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ornate Shinkite Brick");
            SacrificeTotal = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShinkiteBrickOrnateTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
    }
}
