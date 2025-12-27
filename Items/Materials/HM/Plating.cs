using Redemption.Globals;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Plating : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 70, 0);
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ItemType<CyberPlating>())
                .AddTile(TileType<SlayerFabricatorTile>())
                .AddDecraftCondition(RedeConditions.DownedSlayer)
                .Register();
        }
    }
}