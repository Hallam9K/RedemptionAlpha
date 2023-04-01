using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabBackDoor : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Back Door");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabBackDoorTile>(), 0);
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
    public class LabBackDoor2 : ModItem
    {
        public override string Texture => "Redemption/Items/Placeable/Furniture/Lab/LabBackDoor";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Back Door");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabBackDoor2Tile>(), 0);
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 15)
                .AddIngredient(ItemID.Glass, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}