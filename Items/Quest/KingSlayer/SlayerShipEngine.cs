using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Quest.KingSlayer
{
    public class SlayerShipEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ship AFTL Engine");
            /* Tooltip.SetDefault("'Stands for Almost-Faster-Than-Light'" +
            "\n[i:" + ModContent.ItemType<RedemptionRoute>() + "][c/f8f8bc: This item may redeem terrible actions]"); */
            Item.ResearchUnlockCount = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 40;
            Item.questItem = true;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.Quest;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.createTile = ModContent.TileType<SlayerShipEngineTile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 70)
                .AddIngredient(ModContent.ItemType<Plating>(), 8)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 6)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<Plutonium>(), 20)
                .AddIngredient(ModContent.ItemType<Uranium>(), 20)
                .AddIngredient(ItemID.Wire, 50)
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
        }
    }
}
