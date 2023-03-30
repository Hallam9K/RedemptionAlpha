using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Quest.KingSlayer
{
    public class SlayerHullPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ship Hull Plating");

            Item.ResearchUnlockCount = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 44;
            Item.questItem = true;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Quest;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.createTile = ModContent.TileType<SlayerHullPlatingTile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 50)
                .AddIngredient(ModContent.ItemType<Plating>(), 12)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
        }
    }
}