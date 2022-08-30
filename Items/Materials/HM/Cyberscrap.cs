using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Cyberscrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyberscrap");
            Tooltip.SetDefault("'Versatile, and can be used to make anything'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JunkMetalTile>(), 0);
            Item.width = 38;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.ammo = Item.type;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<CyberPlating>())
                .AddCondition(Recipe.Condition.NearLava)
                .Register();
        }
    }
}