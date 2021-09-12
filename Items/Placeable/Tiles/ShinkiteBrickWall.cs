using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<ShinkiteBrickWallTile>();
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<ShinkiteBrick>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}