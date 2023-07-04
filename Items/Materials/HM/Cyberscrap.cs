using Redemption.Globals;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Cyberscrap : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyberscrap");
            // Tooltip.SetDefault("'Versatile, and can be used to make anything'");
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JunkMetalTile>(), 0);
            Item.width = 48;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<CyberPlating>())
                .AddCondition(Condition.NearLava)
                .AddDecraftCondition(RedeConditions.DownedSlayer)
                .Register();
        }
    }
}