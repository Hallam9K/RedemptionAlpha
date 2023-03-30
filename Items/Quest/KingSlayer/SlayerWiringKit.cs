using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Quest.KingSlayer
{
    public class SlayerWiringKit : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ship Wiring Kit");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));

            Item.ResearchUnlockCount = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 28;
            Item.questItem = true;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.rare = ItemRarityID.Quest;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.createTile = ModContent.TileType<SlayerWiringKitTile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 20)
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 2)
                .AddRecipeGroup(RedeRecipe.CopperRecipeGroup, 8)
                .AddIngredient(ItemID.Wire, 15)
                .AddTile(ModContent.TileType<SlayerFabricatorTile>())
                .Register();
        }
    }
}