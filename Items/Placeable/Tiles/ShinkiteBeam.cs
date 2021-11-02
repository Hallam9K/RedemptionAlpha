using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBeam : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShinkiteBeamTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<ShinkiteBrick>())
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
