using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class InfectedCorpse1 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse (Sitting)");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse1Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class InfectedCorpse2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse (Laying on Back)");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse2Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class InfectedCorpse3 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/InfectedCorpse";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse (Laying on Stomach)");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<InfectedCorpse3Tile>(), 0);
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}