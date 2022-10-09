using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class InfectedCorpse1 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpse (Sitting)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse1Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class InfectedCorpse2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpse (Laying on Back)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse2Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class InfectedCorpse3 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corpse (Laying on Stomach)");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse3Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}