using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class CyberChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CyberChairTile>(), 0);
            Item.width = 16;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 400;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Cyberscrap>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}