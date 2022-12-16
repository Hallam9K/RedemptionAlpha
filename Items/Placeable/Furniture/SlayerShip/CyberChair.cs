using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class CyberChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CyberChairTile>(), 0);
            Item.width = 16;
            Item.height = 34;
            Item.maxStack = 9999;
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