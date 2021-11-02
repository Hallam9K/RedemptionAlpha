using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShinkiteBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShinkiteBrickWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
