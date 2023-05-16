using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class SilverwoodSign : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silverwood Danger Sign");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodSignTile>());
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(8)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}