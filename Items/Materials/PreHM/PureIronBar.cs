using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Tiles.Bars;

namespace Redemption.Items.Materials.PreHM
{
    public class PureIronBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 99;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            //Item.consumable = true;
            //Item.createTile = ModContent.TileType<PureIronBarTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("Redemption:IronBar", 2)
                .AddIngredient(ModContent.ItemType<GathicCryoCrystal>())
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
